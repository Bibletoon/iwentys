﻿using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Iwentys.Models.Types;
using Newtonsoft.Json;
using static System.String;

namespace Iwentys.Core.GoogleTableParsing
{
    public class TableParser : ITableParser
    {
        private SheetsService _service;
        private ValueRange _data;
        private TableStringHelper _helper;

        public TableParser(SheetsService service, GoogleTableData tableData)
        {
            _service = service;

            _helper = new TableStringHelper(tableData);
        }

        private void InitDataFromTable()
        {
            if (_data == null)
            {
                var request = _service.Spreadsheets.Values.Get(_helper.Id, _helper.Range);
                _data = request.Execute();
            }
        }

        public List<StudentSubjectScore> GetStudentsList()
        {
            InitDataFromTable();

            var result = new List<StudentSubjectScore>();
            foreach (var row in _data.Values)
            {
                var group = row[_helper.GroupColumnNum];
                var name = row[_helper.NameColumnNum];
                var score = row[_helper.ScoreColumnNum];
                if (group != null && name != null && score != null)
                {
                    var fullName = Join(" ", _helper.NameColumns.Select(c => row[c]));
                    result.Add(new StudentSubjectScore(
                        _helper.GroupDefined ? _helper.GroupName : row[_helper.GroupColumnNum].ToString(),
                        fullName,
                        row[_helper.ScoreColumnNum].ToString()));
                }
                else
                {
                    //TODO: add logging
                }
            }

            return result;
        }
        public string GetStudentsJson()
        {
            var result = GetStudentsList();

            return JsonConvert.SerializeObject(result);
        }
    }
}