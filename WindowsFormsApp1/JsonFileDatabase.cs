﻿using Application;

namespace WindowsFormsApp1
{
    public class JsonFileDatabase : SimpleDataContext
    {
        public JsonFileDatabase(string filePath)
        {
            _data = new NewtonsoftJsonData.Db<Root>(filePath).Data; 

            if (_data == null)
                _data = new Root();
        }
    }
}
