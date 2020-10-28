using Microsoft.Extensions.Logging;
using PuiTranslate.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PuiTranslate.Services
{
    public interface ITranslationDataService
    {
        Task<List<TranslationListEntry>> LoadData(int page = 1);
        Task<List<TranslationListEntry>> FilterData(string filter);
        Task<List<TranslationEntry>> GetDetailRaw(int id);
        Task<TranslationListEntry> GetDetail(int id);
        Task<int> GetNextId();
        Task<bool> UpdateTranslations(EditViewModel model);
    }
    public class TranslationDataService : ITranslationDataService
    {
        private readonly ILogger<TranslationDataService> _logger;
        private readonly ISQLService _sqlService;
        private readonly int _limit;

        public TranslationDataService(ILogger<TranslationDataService> logger, ISQLService sqlService)
        {
            _logger = logger;
            _sqlService = sqlService;

            _limit = 100;
        }

        public async Task<List<TranslationListEntry>> LoadData(int page = 1)
        {
            try
            {
                var offset = _limit * (page - 1);

                var sqlStm = "SELECT i.trid, de.TRLANG AS delang, de.TRPHRASE AS detext, en.TRLANG AS enlang, en.TRPHRASE AS entext FROM ";
                sqlStm += "(SELECT DISTINCT trid FROM profound82.puitransp ORDER BY trid) AS i ";
                sqlStm += "left JOIN profound82.puitransp AS de ON de.TRID = i.trid AND de.TRLANG = 'de_DE' ";
                sqlStm += "left JOIN profound82.puitransp AS en ON en.TRID = i.trid AND en.TRLANG = 'en_EN' ";
                sqlStm += "where i.trid > 10 ";
                sqlStm += $"LIMIT {_limit} offset {offset}";

                var res = await _sqlService.Query<TranslationEntry>(sqlStm);
                return _mapEntries(res);
            } catch(Exception ex)
            {
                throw new Exception($"Error loading translation data with page {page}: {ex.Message}");
            }

        }

        public async Task<List<TranslationListEntry>> FilterData(string filter)
        {
            try
            {
                var sqlStm = "SELECT i.trid, de.TRLANG AS delang, de.TRPHRASE AS detext, en.TRLANG AS enlang, en.TRPHRASE AS entext FROM ";
                sqlStm += "(SELECT DISTINCT trid FROM profound82.puitransp ORDER BY trid) AS i ";
                sqlStm += "left JOIN profound82.puitransp AS de ON de.TRID = i.trid AND de.TRLANG = 'de_DE' ";
                sqlStm += "left JOIN profound82.puitransp AS en ON en.TRID = i.trid AND en.TRLANG = 'en_EN' ";
                sqlStm += "where i.trid > 10 ";
                sqlStm += $"AND (UPPER(de.TRPHRASE) LIKE UPPER('%{filter}%') OR UPPER(en.TRPHRASE) LIKE UPPER('%{filter}%') OR i.TRID LIKE '{filter}%') ";
                sqlStm += "LIMIT 100";

                var res = await _sqlService.Query<TranslationEntry>(sqlStm);
                return _mapEntries(res);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading translation data with filter {filter}: {ex.Message}");
            }
        }

        public async Task<List<TranslationEntry>> GetDetailRaw(int id)
        {
            try
            {

                var sqlStm = "SELECT i.trid, de.TRLANG AS delang, de.TRPHRASE AS detext, en.TRLANG AS enlang, en.TRPHRASE AS entext FROM ";
                sqlStm += "(SELECT DISTINCT trid FROM profound82.puitransp ORDER BY trid) AS i ";
                sqlStm += "left JOIN profound82.puitransp AS de ON de.TRID = i.trid AND de.TRLANG = 'de_DE' ";
                sqlStm += "left JOIN profound82.puitransp AS en ON en.TRID = i.trid AND en.TRLANG = 'en_EN' ";
                sqlStm += $"where i.trid between {id}1 and {id}9 ";

                return await _sqlService.Query<TranslationEntry>(sqlStm);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading raw translation data for id {id}: {ex.Message}");
            }
        }

        public async Task<TranslationListEntry> GetDetail(int id)
        {
            try
            {
                var res = await GetDetailRaw(id);
                var mapped = _mapEntries(res);
                return mapped.First();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading translation data for id {id}: {ex.Message}");
            }
        }

        public async Task<int> GetNextId()
        {
            try
            {
                var sqlStm = "SELECT max(trid) as newid FROM profound82.puitransp";
                var res = await _sqlService.Query<IntResponse>(sqlStm);
                var id = res.First();
                var strid = id.NewId.ToString();
                var grpid = Convert.ToInt32(strid.Substring(0, strid.Length - 1));
                return grpid + 1;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading the next id: {ex.Message}");
            }
        }

        public async Task<bool> UpdateTranslations(EditViewModel model)
        {
            try
            {
                var sql = "MERGE INTO profound82.puitransp AS target ";
                sql += "USING (VALUES ";

                sql += $"({model.GrpId}1, 'de_DE', '{model.DeLong}'), ";
                sql += $"({model.GrpId}2, 'de_DE', '{model.DeMiddle}'), ";
                sql += $"({model.GrpId}3, 'de_DE', '{model.DeShort}'), ";
                sql += $"({model.GrpId}1, 'en_EN', '{model.EnLong}'), ";
                sql += $"({model.GrpId}2, 'en_EN', '{model.EnMiddle}'), ";
                sql += $"({model.GrpId}3, 'en_EN', '{model.EnShort}') ";

                sql += ") AS src (c1, c2, c3) ";
                sql += "ON (target.trid = src.c1 AND TARGET.TRLANG = src.c2) ";
                sql += "WHEN MATCHED THEN ";
                sql += "UPDATE SET target.TRLANG = src.c2, target.TRPHRASE = src.c3 ";
                sql += "WHEN NOT MATCHED THEN ";
                sql += "INSERT (trid, TRLANG, TRPHRASE) VALUES(src.c1, src.c2, src.c3) with none";

                return await _sqlService.Excute(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating/updating translations: {ex.Message}");
            }
        }

        //public async Task<bool> UpdateTranslations(EditViewModel model)
        //{
        //    try
        //    {
        //        var sqlStm = "INSERT INTO profound82.puitransp (trid, TRLANG, TRPHRASE) ";
        //        sqlStm += $"VALUES ({model.GrpId}1, 'de_DE', '{model.DeLong}'), ";
        //        sqlStm += $"VALUES ({model.GrpId}2, 'de_DE', '{model.DeMiddle}'), ";
        //        sqlStm += $"VALUES ({model.GrpId}3, 'de_DE', '{model.DeShort}'), ";
        //        sqlStm += $"VALUES ({model.GrpId}1, 'en_EN', '{model.EnLong}'), ";
        //        sqlStm += $"VALUES ({model.GrpId}2, 'en_EN', '{model.EnMiddle}'), ";
        //        sqlStm += $"VALUES ({model.GrpId}3, 'en_EN', '{model.EnShort}'), ";

        //        return await _sqlService.Excute(sqlStm);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error creating/updating translations: {ex.Message}");
        //    }
        //}

        //public async Task<bool> UpdateTranslation(EditViewModel model)
        //{
        //    try
        //    {
        //        var sql = "MERGE INTO profound82.puitransp AS target ";
        //        sql += "USING (VALUES ";

        //        sql += $"({model.GrpId}1, 'de_DE', '{model.DeLong}'), ";
        //        sql += $"({model.GrpId}2, 'de_DE', '{model.DeMiddle}'), ";
        //        sql += $"({model.GrpId}3, 'de_DE', '{model.DeShort}'), ";
        //        sql += $"({model.GrpId}1, 'en_EN', '{model.EnLong}'), ";
        //        sql += $"({model.GrpId}2, 'en_EN', '{model.EnMiddle}'), ";
        //        sql += $"({model.GrpId}3, 'en_EN', '{model.EnShort}'), ";

        //        sql += ") AS source (c1, c2, c3) ";
        //        sql += "ON (target.trid = source.c1) ";
        //        sql += "WHEN MATCHED THEN ";
        //        sql += "UPDATE SET target.TRLANG = source.c2, target.TRPHRASE = source.c3 ";
        //        sql += "WHEN NOT MATCHED THEN ";
        //        sql += "INSERT (trid, TRLANG, TRPHRASE) VALUES(source.c1, source.c2, source.c3) ";

        //        return await _sqlService.Excute(sql);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error loading updating translation: {ex.Message}");
        //    }
        //}

        private List<TranslationListEntry> _mapEntries(List<TranslationEntry> entries)
        {
            var items = new List<TranslationListEntry>();

            foreach(var e in entries)
            {
                var ce = e;
                var id = e.Id.ToString();
                ce.GroupId = Convert.ToInt32(id.Substring(0, id.Length - 1));

                var grp = items.Where(x => x.Id == ce.GroupId).FirstOrDefault();
                if(grp == null)
                {
                    grp = new TranslationListEntry();
                    grp.Id = ce.GroupId;
                    items.Add(grp);
                }

                var de = grp.LangItems.Where(x => x.LangCode == "de_DE").FirstOrDefault();
                if (de == null)
                {
                    de = new LanguageItem();
                    de.LangCode = "de_DE";
                    de.LangText = "Deutsch";
                    grp.LangItems.Add(de);
                }

                var en = grp.LangItems.Where(x => x.LangCode == "en_EN").FirstOrDefault();
                if (en == null)
                {
                    en = new LanguageItem();
                    en.LangCode = "en_EN";
                    en.LangText = "Englisch";
                    grp.LangItems.Add(en);
                }

                switch (id.Last())
                {
                    case '3':
                        de.Short = e.DeText;
                        en.Short = e.EnText;
                        break;
                    case '2':
                        de.Middle = e.DeText;
                        en.Middle = e.EnText;
                        break;
                    case '1':
                        de.Long = e.DeText;
                        en.Long = e.EnText;
                        break;
                }
            }

            foreach(var i in items)
            {
                var de = i.LangItems.Where(x => x.LangCode == "de_DE").FirstOrDefault();
                var en = i.LangItems.Where(x => x.LangCode == "en_EN").FirstOrDefault();

                if(de != null)
                {
                    if (!string.IsNullOrEmpty(de.Long))
                    {
                        i.DescriptionDE = de.Long;
                    }
                    if (string.IsNullOrEmpty(i.DescriptionDE) && !string.IsNullOrEmpty(de.Middle))
                    {
                        i.DescriptionDE = de.Middle;
                    }
                    if (string.IsNullOrEmpty(i.DescriptionDE) &&  !string.IsNullOrEmpty(de.Short))
                    {
                        i.DescriptionDE = de.Short;
                    }
                }

                if (en != null)
                {
                    if (!string.IsNullOrEmpty(en.Long))
                    {
                        i.DescriptionEN = en.Long;
                    }
                    if (string.IsNullOrEmpty(i.DescriptionEN) && !string.IsNullOrEmpty(en.Middle))
                    {
                        i.DescriptionEN = en.Middle;
                    }
                    if (string.IsNullOrEmpty(i.DescriptionEN) && !string.IsNullOrEmpty(en.Short))
                    {
                        i.DescriptionEN = en.Short;
                    }
                }
            }

            return items;
        }
    }
}
