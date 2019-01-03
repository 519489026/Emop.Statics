using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Common
{
    public class ConvertTableToList
    {
        public static T GetEntity<T>(DataTable table) where T : new()
        {
            T entity = new T();
            if (table == null || table.Rows.Count == 0)
            {
                return default(T);
            }

            foreach (var item in entity.GetType().GetProperties())
            {
                if (table.Columns.Contains(item.Name))
                {
                    if (DBNull.Value != table.Rows[0][item.Name])
                    {
                        item.SetValue(entity, Convert.ChangeType(table.Rows[0][item.Name], item.PropertyType), null);
                    }

                }
            }
            return entity;
        }

        public static IList<T> GetEntities<T>(DataTable table) where T : new()
        {
            IList<T> entities = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    item.SetValue(entity, Convert.ChangeType(row[item.Name], item.PropertyType), null);
                }
                entities.Add(entity);
            }
            return entities;
        }
    }
}
