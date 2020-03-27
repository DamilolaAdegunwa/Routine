using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace BlockSms.Core.Log4Net
{
    /// <summary>
    // <appender name="ADONetAppender" type="BlockSms.Core.Log4Net.AdoNetAppender, BlockSms.Core">
    //  <connectionType value = "System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
    //  < connectionString value="data source=.;initial catalog=Logs;integrated security=false;persist security info=True;User ID=sa;Password=pass" />
    //  <!--插入数据库的sql语句-->
    //  <commandText value = "INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message]) VALUES (@log_date, @thread, @log_level, @logger, @message)" />
    //  < parameter >
    //    < parameterName value="@log_date" />
    //    <dbType value = "DateTime" />
    //    < layout type="log4net.Layout.PatternLayout" value="%date{yyyy'-'MM'-'dd HH':'mm':'ss'.'fff}" />
    //  </parameter>
    //  <parameter>
    //    <parameterName value = "@thread" />
    //    < dbType value="String" />
    //    <size value = "255" />
    //    < layout type="log4net.Layout.PatternLayout" value="%thread" />
    //  </parameter>
    //  <parameter>
    //    <parameterName value = "@log_level" />
    //    < dbType value="String" />
    //    <size value = "50" />
    //    < layout type="log4net.Layout.PatternLayout" value="%level" />
    //  </parameter>
    //  <parameter>
    //    <parameterName value = "@logger" />
    //    < dbType value="String" />
    //    <size value = "255" />
    //    < layout type="log4net.Layout.PatternLayout" value="%logger" />
    //  </parameter>
    //  <parameter>
    //    <parameterName value = "@message" />
    //    < dbType value="String" />
    //    <size value = "4000" />
    //    < layout type="log4net.Layout.PatternLayout" value="%message" />
    //  </parameter>
    //</appender>
    /// </summary>
    public class AdoNetAppender : BufferingAppenderSkeleton
    {
        private readonly static Type declaringType = typeof(AdoNetAppender);
        /// <summary>
		/// Database connection string.
		/// </summary>
		private string m_connectionString;
        /// <summary>
        /// The appSettings key from App.Config that contains the connection string.
        /// </summary>
        private string m_appSettingsKey;
        /// <summary>
        /// String type name of the <see cref="IDbConnection"/> type name.
        /// </summary>
        private string m_connectionType;
        /// <summary>
		/// The text of the command.
		/// </summary>
		private string m_commandText;
        /// <summary>
		/// The command type.
		/// </summary>
		private CommandType m_commandType;
        /// <summary>
		/// Indicates whether to use transactions when writing to the database.
		/// </summary>
		private bool m_useTransactions;
        /// <summary>
		/// The security context to use for privileged calls
		/// </summary>
		private SecurityContext m_securityContext;
        /// <summary>
		/// Indicates whether to reconnect when a connection is lost.
		/// </summary>
		private bool m_reconnectOnError;
        /// <summary>
        /// The <see cref="IDbConnection" /> that will be used
        /// to insert logging events into a database.
        /// </summary>
        private IDbConnection m_dbConnection;
        protected ArrayList m_parameters;
        public AdoNetAppender()
        {
            ConnectionType = typeof(SqlConnection).AssemblyQualifiedName;
            UseTransactions = true;
            CommandType = System.Data.CommandType.Text;
            m_parameters = new ArrayList();
            ReconnectOnError = false;
        }
        public string ConnectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }
        public string AppSettingsKey
        {
            get { return m_appSettingsKey; }
            set { m_appSettingsKey = value; }
        }
        public string ConnectionType
        {
            get { return m_connectionType; }
            set { m_connectionType = value; }
        }
        public string CommandText
        {
            get { return m_commandText; }
            set { m_commandText = value; }
        }
        public CommandType CommandType
        {
            get { return m_commandType; }
            set { m_commandType = value; }
        }
        public bool UseTransactions
        {
            get { return m_useTransactions; }
            set { m_useTransactions = value; }
        }
        public SecurityContext SecurityContext
        {
            get { return m_securityContext; }
            set { m_securityContext = value; }
        }
        public bool ReconnectOnError
        {
            get { return m_reconnectOnError; }
            set { m_reconnectOnError = value; }
        }
        protected IDbConnection Connection
        {
            get { return m_dbConnection; }
            set { m_dbConnection = value; }
        }

        override public void ActivateOptions()
        {
            base.ActivateOptions();

            if (SecurityContext == null)
            {
                SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
            }

            InitializeDatabaseConnection();
        }

        override protected void OnClose()
        {
            base.OnClose();
            DiposeConnection();
        }
        override protected void SendBuffer(LoggingEvent[] events)
        {
            if (ReconnectOnError && (Connection == null || Connection.State != ConnectionState.Open))
            {
                LogLog.Debug(declaringType, "Attempting to reconnect to database. Current Connection State: " + ((Connection == null) ? SystemInfo.NullText : Connection.State.ToString()));

                InitializeDatabaseConnection();
            }

            // Check that the connection exists and is open
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                if (UseTransactions)
                {
                    // NJC - Do this on 2 lines because it can confuse the debugger
                    using (IDbTransaction dbTran = Connection.BeginTransaction())
                    {
                        try
                        {
                            SendBuffer(dbTran, events);
                            dbTran.Commit();
                        }
                        catch (Exception ex)
                        {
                            // rollback the transaction
                            try
                            {
                                dbTran.Rollback();
                            }
                            catch (Exception)
                            {
                                // Ignore exception
                            }
                            // Can't insert into the database. That's a bad thing
                            ErrorHandler.Error("Exception while writing to database", ex);
                        }
                    }
                }
                else
                {
                    // Send without transaction
                    SendBuffer(null, events);
                }
            }
        }
        public void AddParameter(AdoNetAppenderParameter parameter)
        {
            m_parameters.Add(parameter);
        }
        virtual protected void SendBuffer(IDbTransaction dbTran, LoggingEvent[] events)
        {
            // string.IsNotNullOrWhiteSpace() does not exist in ancient .NET frameworks
            if (CommandText != null && CommandText.Trim() != "")
            {
                using (IDbCommand dbCmd = Connection.CreateCommand())
                {
                    // Set the command string
                    dbCmd.CommandText = CommandText;

                    // Set the command type
                    dbCmd.CommandType = CommandType;
                    // Send buffer using the prepared command object
                    if (dbTran != null)
                    {
                        dbCmd.Transaction = dbTran;
                    }
                    // prepare the command, which is significantly faster
                    dbCmd.Prepare();
                    // run for all events
                    foreach (LoggingEvent e in events)
                    {
                        // clear parameters that have been set
                        dbCmd.Parameters.Clear();

                        // Set the parameter values
                        foreach (AdoNetAppenderParameter param in m_parameters)
                        {
                            param.Prepare(dbCmd);
                            param.FormatValue(dbCmd, e);
                        }

                        // Execute the query
                        dbCmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                // create a new command
                using (IDbCommand dbCmd = Connection.CreateCommand())
                {
                    if (dbTran != null)
                    {
                        dbCmd.Transaction = dbTran;
                    }
                    // run for all events
                    foreach (LoggingEvent e in events)
                    {
                        // Get the command text from the Layout
                        string logStatement = GetLogStatement(e);

                        LogLog.Debug(declaringType, "LogStatement [" + logStatement + "]");

                        dbCmd.CommandText = logStatement;
                        dbCmd.ExecuteNonQuery();
                    }
                }
            }
        }
        virtual protected string GetLogStatement(LoggingEvent logEvent)
        {
            if (Layout == null)
            {
                ErrorHandler.Error("AdoNetAppender: No Layout specified.");
                return "";
            }
            else
            {
                StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
                Layout.Format(writer, logEvent);
                return writer.ToString();
            }
        }
        virtual protected IDbConnection CreateConnection(Type connectionType, string connectionString)
        {
            IDbConnection connection = (IDbConnection)Activator.CreateInstance(connectionType);
            connection.ConnectionString = connectionString;
            return connection;
        }
        virtual protected string ResolveConnectionString(out string connectionStringContext)
        {
            if (ConnectionString != null && ConnectionString.Length > 0)
            {
                connectionStringContext = "ConnectionString";
                return ConnectionString;
            }
            if (AppSettingsKey != null && AppSettingsKey.Length > 0)
            {
                connectionStringContext = "AppSettingsKey";
                string appSettingsConnectionString = SystemInfo.GetAppSetting(AppSettingsKey);
                if (appSettingsConnectionString == null || appSettingsConnectionString.Length == 0)
                {
                    throw new LogException("Unable to find [" + AppSettingsKey + "] AppSettings key.");
                }
                return appSettingsConnectionString;
            }

            connectionStringContext = "Unable to resolve connection string from ConnectionString, ConnectionStrings, or AppSettings.";
            return string.Empty;
        }
        virtual protected Type ResolveConnectionType()
        {
            try
            {
                return SystemInfo.GetTypeFromString(GetType().Assembly, ConnectionType, true, false);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Failed to load connection type [" + ConnectionType + "]", ex);
                throw;
            }
        }
        private void InitializeDatabaseConnection()
        {
            string connectionStringContext = "Unable to determine connection string context.";
            string resolvedConnectionString = string.Empty;

            try
            {
                DiposeConnection();

                // Set the connection string
                resolvedConnectionString = ResolveConnectionString(out connectionStringContext);

                Connection = CreateConnection(ResolveConnectionType(), resolvedConnectionString);

                using (SecurityContext.Impersonate(this))
                {
                    // Open the database connection
                    Connection.Open();
                }
            }
            catch (Exception e)
            {
                // Sadly, your connection string is bad.
                ErrorHandler.Error("Could not open database connection [" + resolvedConnectionString + "]. Connection string context [" + connectionStringContext + "].", e);

                Connection = null;
            }
        }
        private void DiposeConnection()
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Close();
                }
                catch (Exception ex)
                {
                    LogLog.Warn(declaringType, "Exception while disposing cached connection object", ex);
                }
                Connection = null;
            }
        }
    }
    public class AdoNetAppenderParameter
    {
        /// <summary>
		/// The name of this parameter.
		/// </summary>
		private string m_parameterName;
        /// <summary>
        /// The database type for this parameter.
        /// </summary>
        private DbType m_dbType;
        /// <summary>
        /// Flag to infer type rather than use the DbType
        /// </summary>
        private bool m_inferType = true;
        /// <summary>
        /// The precision for this parameter.
        /// </summary>
        private byte m_precision;
        /// <summary>
        /// The scale for this parameter.
        /// </summary>
        private byte m_scale;
        /// <summary>
        /// The size for this parameter.
        /// </summary>
        private int m_size;
        /// <summary>
        /// The <see cref="IRawLayout"/> to use to render the
        /// logging event into an object for this parameter.
        /// </summary>
        private IRawLayout m_layout;
        public AdoNetAppenderParameter()
        {
            Precision = 0;
            Scale = 0;
            Size = 0;
        }
        public string ParameterName
        {
            get { return m_parameterName; }
            set { m_parameterName = value; }
        }
        public DbType DbType
        {
            get { return m_dbType; }
            set
            {
                m_dbType = value;
                m_inferType = false;
            }
        }
        public byte Precision
        {
            get { return m_precision; }
            set { m_precision = value; }
        }
        public byte Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }
        public int Size
        {
            get { return m_size; }
            set { m_size = value; }
        }
        public IRawLayout Layout
        {
            get { return m_layout; }
            set { m_layout = value; }
        }

        virtual public void Prepare(IDbCommand command)
        {
            // Create a new parameter
            IDbDataParameter param = command.CreateParameter();

            // Set the parameter properties
            param.ParameterName = ParameterName;

            if (!m_inferType)
            {
                param.DbType = DbType;
            }
            if (Precision != 0)
            {
                param.Precision = Precision;
            }
            if (Scale != 0)
            {
                param.Scale = Scale;
            }
            if (Size != 0)
            {
                param.Size = Size;
            }

            // Add the parameter to the collection of params
            command.Parameters.Add(param);
        }
        virtual public void FormatValue(IDbCommand command, LoggingEvent loggingEvent)
        {
            // Lookup the parameter
            IDbDataParameter param = (IDbDataParameter)command.Parameters[ParameterName];

            // Format the value
            object formattedValue = Layout.Format(loggingEvent);

            // If the value is null then convert to a DBNull
            if (formattedValue == null)
            {
                formattedValue = DBNull.Value;
            }

            param.Value = formattedValue;
        }
    }
}
