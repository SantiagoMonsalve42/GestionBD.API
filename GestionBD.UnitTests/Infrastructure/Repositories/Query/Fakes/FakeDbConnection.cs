using System.Collections;
using System.Data;
using System.Data.Common;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Query.Fakes;

internal sealed class FakeDbConnection : DbConnection
{
    private ConnectionState _state = ConnectionState.Closed;

    public object? ScalarResult { get; set; }
    public bool ThrowOnExecute { get; set; }
    public override string ConnectionString { get; set; } = string.Empty;
    public override string Database => "FakeDb";
    public override string DataSource => "FakeSource";
    public override string ServerVersion => "1.0";
    public override ConnectionState State => _state;

    public override void ChangeDatabase(string databaseName)
    {
    }

    public override void Close()
    {
        _state = ConnectionState.Closed;
    }

    public override void Open()
    {
        _state = ConnectionState.Open;
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
        throw new NotSupportedException();

    protected override DbCommand CreateDbCommand() =>
        new FakeDbCommand(this);

    private sealed class FakeDbCommand : DbCommand
    {
        private readonly FakeDbConnection _connection;
        private readonly FakeDbParameterCollection _parameters = new();

        public FakeDbCommand(FakeDbConnection connection)
        {
            _connection = connection;
        }

        public override string CommandText { get; set; } = string.Empty;
        public override int CommandTimeout { get; set; } = 30;
        public override CommandType CommandType { get; set; } = CommandType.Text;
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; } = UpdateRowSource.None;

        protected override DbConnection DbConnection
        {
            get => _connection;
            set { }
        }

        protected override DbParameterCollection DbParameterCollection => _parameters;
        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            ThrowIfNeeded();
            return 0;
        }

        public override object? ExecuteScalar()
        {
            ThrowIfNeeded();
            return _connection.ScalarResult;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            ThrowIfNeeded();
            return new FakeDbDataReader();
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter() => new FakeDbParameter();

        private void ThrowIfNeeded()
        {
            if (_connection.ThrowOnExecute)
                throw new InvalidOperationException("Test exception");
        }
    }

    private sealed class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = [];

        public override int Count => _parameters.Count;
        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value!);
            }
        }

        public override void Clear() => _parameters.Clear();
        public override bool Contains(object value) => _parameters.Contains((DbParameter)value);
        public override bool Contains(string value) => _parameters.Any(p => p.ParameterName == value);
        public override void CopyTo(Array array, int index) => _parameters.ToArray().CopyTo(array, index);
        public override IEnumerator GetEnumerator() => _parameters.GetEnumerator();
        public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _parameters.FindIndex(p => p.ParameterName == parameterName);
        public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _parameters.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _parameters.RemoveAt(index);
        public override void RemoveAt(string parameterName) => _parameters.RemoveAt(IndexOf(parameterName));

        protected override DbParameter GetParameter(int index) => _parameters[index];
        protected override DbParameter GetParameter(string parameterName) => _parameters[IndexOf(parameterName)];
        protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index < 0)
            {
                _parameters.Add(value);
                return;
            }
            _parameters[index] = value;
        }
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; } = string.Empty;
        public override string SourceColumn { get; set; } = string.Empty;
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }

        public override void ResetDbType()
        {
        }
    }

    private sealed class FakeDbDataReader : DbDataReader
    {
        public override int FieldCount => 0;
        public override bool HasRows => false;
        public override bool IsClosed => false;
        public override int RecordsAffected => 0;

        public override bool Read() => false;
        public override bool NextResult() => false;
        public override int Depth => 0;
        public override object this[int ordinal] => throw new IndexOutOfRangeException();
        public override object this[string name] => throw new IndexOutOfRangeException();

        public override bool GetBoolean(int ordinal) => throw new IndexOutOfRangeException();
        public override byte GetByte(int ordinal) => throw new IndexOutOfRangeException();
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => 0;
        public override char GetChar(int ordinal) => throw new IndexOutOfRangeException();
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => 0;
        public override string GetDataTypeName(int ordinal) => string.Empty;
        public override DateTime GetDateTime(int ordinal) => throw new IndexOutOfRangeException();
        public override decimal GetDecimal(int ordinal) => throw new IndexOutOfRangeException();
        public override double GetDouble(int ordinal) => throw new IndexOutOfRangeException();
        public override Type GetFieldType(int ordinal) => typeof(object);
        public override float GetFloat(int ordinal) => throw new IndexOutOfRangeException();
        public override Guid GetGuid(int ordinal) => throw new IndexOutOfRangeException();
        public override short GetInt16(int ordinal) => throw new IndexOutOfRangeException();
        public override int GetInt32(int ordinal) => throw new IndexOutOfRangeException();
        public override long GetInt64(int ordinal) => throw new IndexOutOfRangeException();
        public override string GetName(int ordinal) => string.Empty;
        public override int GetOrdinal(string name) => -1;
        public override string GetString(int ordinal) => throw new IndexOutOfRangeException();
        public override object GetValue(int ordinal) => throw new IndexOutOfRangeException();
        public override int GetValues(object[] values) => 0;
        public override bool IsDBNull(int ordinal) => true;
        public override IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();
    }
}