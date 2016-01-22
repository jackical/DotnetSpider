﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class MysqlFileRepository<T> where T : new()
	{
		private readonly string _path;
		private readonly Type _type;
		private readonly List<PropertyInfo> _propertyInfos;

		public MysqlFileRepository(string path)
		{
			if (!File.Exists(path))
			{
				throw new SpiderExceptoin("File does not exist.");
			}
			_path = path;
			_type = typeof(T);
			_propertyInfos = _type.GetProperties(BindingFlags.Public).Where(p => p.GetCustomAttribute<NonStored>() == null).ToList();
		}

		public List<T> GetAll()
		{
			List<T> list = new List<T>();
			using (StreamReader reader = new StreamReader(File.OpenRead(_path)))
			{
				string line;
				while (!string.IsNullOrEmpty(line = reader.ReadLine()))
				{
					string[] columns = line.Split(',');
					if (columns.Length != _propertyInfos.Count)
					{
						throw new SpiderExceptoin("Count of columns is not match: " + line);
					}
					T t = new T();

					for (int i = 0; i < _propertyInfos.Count; ++i)
					{
						_propertyInfos[i].SetValue(t,columns[i]);
					}
					list.Add(t);
				}
			}
			return list;
		}

	}
}