using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MyService.Logic.Sql;

public class BaseLogic
{
    private readonly DbContext _dbContext;

    public BaseLogic(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 获取指定类型的所有实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>所有实体的列表</returns>
    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    /// <summary>
    /// 根据 ID 获取指定类型的实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="id">实体的 ID</param>
    /// <returns>指定 ID 的实体，如果不存在则返回 null</returns>
    public async Task<T> GetByIdAsync<T>(int id) where T : class
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    /// <summary>
    /// 根据条件获取指定类型的实体列表
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="predicate">查询条件</param>
    /// <returns>满足条件的实体列表</returns>
    public async Task<List<T>> GetByFuncAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    /// <summary>
    /// 添加一个实体到数据库中
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">要添加的实体</param>
    /// <returns>表示异步添加操作的 Task</returns>
    public async Task AddAsync<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 更新一个实体在数据库中的数据
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">要更新的实体</param>
    /// <returns>表示异步更新操作的 Task</returns>
    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 从数据库中删除一个实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">要删除的实体</param>
    /// <returns>表示异步删除操作的 Task</returns>
    public async Task DeleteAsync<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}