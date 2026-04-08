using Dapper;

namespace DotnetAPI.Data
{
    public interface IDataContextDapper
    {
        IEnumerable<T> LoadData<T>(string sql);
        T LoadDataSingle<T>(string sql);
        int ExecuteSqlWithRowCount(string sql);
        bool ExecuteSql(string sql);
        bool ExecuteSqlWithParameter(string sql, DynamicParameters parameters);
        IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters);
        T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters);
    }
}
