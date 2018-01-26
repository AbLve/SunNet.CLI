using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace SyncTrainingRecord.DatabaseService
{
    public class HelperFillEntity<T> where T : new()
    {
        #region Private Methods
        private static object SetNull(PropertyInfo propertyInfo)
        {
            switch (propertyInfo.PropertyType.ToString())
            {
                case "System.Int16":
                    return Int16.MinValue;

                case "System.Int32":
                    return Int32.MinValue;

                case "System.Int64":
                    return Int64.MinValue;

                case "System.Single":
                    return Single.MinValue;

                case "System.Double":
                    return Double.MinValue;

                case "System.Decimal":
                    return Decimal.MinValue;

                case "System.DateTime":
                    return DateTime.MinValue;

                case "System.String":
                    return String.Empty;

                case "System.Char":
                    return String.Empty;

                case "System.Boolean":
                    return false;

                case "System.Guid":
                    return Guid.Empty;

                default:
                    Type type = propertyInfo.PropertyType;
                    if (type.IsEnum == true)
                    {
                        Array enumValues = System.Enum.GetValues(type);
                        Array.Sort(enumValues);
                        return Enum.Parse(type, Convert.ToString(enumValues.GetValue(0)));
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        private static List<string> GetColums(IDataReader dr)
        {
            List<string> columns = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                columns.Add(dr.GetName(i).ToLower());
            }

            return columns;
        }

        private static List<PropertyInfo> GetProperties()
        {
            T t = new T();
            string key = t.GetType().FullName;
            List<PropertyInfo> properties = new List<PropertyInfo>();
            properties = new List<PropertyInfo>();
            foreach (PropertyInfo p in t.GetType().GetProperties())
            {
                properties.Add(p);
            }
            return properties;
        }

        private static void SetValue(T t, IDataReader dr, List<string> columns, PropertyInfo p)
        {
            if (p.CanWrite == true)
            {
                object propertyValue = SetNull(p);
                if (columns.Contains(p.Name.ToLower()) == true)
                {
                    object dbValue = dr[p.Name.ToLower()];
                    if (Convert.IsDBNull(dbValue) == true)
                    {
                        p.SetValue(t, propertyValue, null);
                    }
                    else
                    {
                        Type propertyType = p.PropertyType;
                        if (propertyType.IsEnum == true)
                        {
                            string enumValue = Convert.ToString(dbValue);
                            p.SetValue(t, System.Enum.Parse(propertyType, enumValue), null);
                        }
                        else
                        {
                            //if (propertyType.Name != "Guid")
                            //    p.SetValue(t, Convert.ChangeType(dbValue, propertyType), null);
                            //else
                            //    p.SetValue(t, new Guid(dbValue.ToString()), null);
                            if (propertyType.Equals(typeof(Boolean)))
                            {
                                p.SetValue(t, Convert.ToBoolean(dbValue), null);
                            }
                            else if (propertyType.Equals(typeof(System.Single)))
                            {
                                p.SetValue(t, Convert.ToSingle(dbValue), null);
                            }
                            else if (propertyType.Equals(typeof(Nullable<bool>)))
                            {
                                Nullable<bool> tb = null;
                                if (dbValue != DBNull.Value)
                                    tb = Convert.ToBoolean(dbValue);
                                p.SetValue(t, tb, null);
                            }
                            else if (propertyType.Equals(typeof(Nullable<DateTime>)))
                            {
                                Nullable<DateTime> td = null;
                                if (dbValue != DBNull.Value)
                                    td = Convert.ToDateTime(dbValue);
                                p.SetValue(t, td, null);
                            }
                            else if (propertyType.Equals(typeof(Nullable<int>)))
                            {
                                Nullable<int> td = null;
                                if (dbValue != DBNull.Value)
                                    td = Convert.ToInt32(dbValue);
                                p.SetValue(t, td, null);
                            }
                            else if (propertyType.Equals(typeof(Nullable<decimal>)))
                            {
                                Nullable<decimal> td = null;
                                if (dbValue != DBNull.Value)
                                    td = Convert.ToDecimal(dbValue);
                                p.SetValue(t, td, null);
                            }
                            else if (propertyType.Equals(typeof(Guid)))
                            {
                                Guid gd = new Guid();
                                if (dbValue != DBNull.Value)
                                    gd = new Guid(Convert.ToString(dbValue));
                                p.SetValue(t, gd, null);
                            }
                            else
                            {
                                if (IsGuid(dbValue))
                                {
                                    Guid gd = Guid.Empty;
                                    try
                                    {
                                        gd = (Guid)dbValue;
                                    }
                                    catch
                                    {
                                        gd = Guid.Empty;
                                    }
                                    if (gd.Equals(Guid.Empty))
                                    {
                                        if (dbValue == null || dbValue == DBNull.Value)
                                            ;
                                        else
                                            gd = new Guid(Convert.ToString(dbValue));
                                    }

                                    try
                                    {
                                        p.SetValue(t, gd, null);
                                    }
                                    catch
                                    {
                                        p.SetValue(t, gd.ToString(), null);
                                    }
                                }
                                else
                                {
                                    p.SetValue(t, Convert.ChangeType(dbValue, propertyType), null);
                                }
                            }
                         }
                    }
                }
            }
        }

        private static bool IsGuid(object o)
        {
            if (o is Guid)
                return true;

            try
            {
                Guid g = new Guid(Convert.ToString(o));
                return true;
            }
            catch
            {
                return false;
            }
        }
       #endregion

        public static T FillEntity(IDataReader dr, bool isClosedReader)
        {
            T t = new T();
            while (dr.Read())
            {
                List<string> columns = new List<string>();
                columns = GetColums(dr);

                List<PropertyInfo> properties = new List<PropertyInfo>();
                properties = GetProperties();

                foreach (PropertyInfo p in properties)
                {
                    SetValue(t, dr, columns, p);
                }
            }

            if (isClosedReader == true)
            {
                if (dr != null && dr.IsClosed == false)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            return t;
        }

        public static T FillEntity(IDataReader dr)
        {
            return FillEntity(dr, true);
        }

        public static List<T> FillEntityList(IDataReader dr, bool isClosedReader)
        {
            List<T> list = new List<T>();
            T t = new T();

            List<string> columns = new List<string>();
            columns = GetColums(dr);

            List<PropertyInfo> properties = new List<PropertyInfo>();
            properties = GetProperties();

            while (dr.Read())
            {
                t = new T();
                foreach (PropertyInfo p in properties)
                {
                    SetValue(t, dr, columns, p);
                }

                list.Add(t);
            }

            if (isClosedReader == true)
            {
                if (dr != null && dr.IsClosed == false)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return list;
        }

        public static List<T> FillEntityList(IDataReader dr)
        {
            return FillEntityList(dr, true);
        }
    }
}
