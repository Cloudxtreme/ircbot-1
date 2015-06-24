using System;
using System.Linq.Expressions;

namespace IrcBot.Database.Repositories
{
    public interface IQueryObject<T>
    {
        Expression<Func<T, bool>> Query();

        Expression<Func<T, bool>> And(Expression<Func<T, bool>> query);

        Expression<Func<T, bool>> And(IQueryObject<T> queryObject);

        Expression<Func<T, bool>> Or(Expression<Func<T, bool>> query);

        Expression<Func<T, bool>> Or(IQueryObject<T> queryObject);
    }
}
