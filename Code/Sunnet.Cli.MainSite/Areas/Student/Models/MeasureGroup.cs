using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.MainSite.Areas.Student.Models
{
    public class MeasureWaveModel
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("otherInfo")]
        public string TheOtherLanguageName { get; set; }
        [JsonIgnore]
        public string Waves
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                if (_applyToWave == null)
                    _applyToWave = new List<int>();
                _applyToWave.AddRange(value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse));
            }
        }

        private List<int> _applyToWave;

        [JsonProperty("applyToWave")]
        public List<int> ApplyToWave
        {
            get { return _applyToWave; }
        }
    }


    public class MeasureGroup
    {
        private static List<MeasureGroup> Parse(List<MeasureHeaderModel> measures, List<MeasureHeaderModel> parentMeasures)
        {
            var measureJson = new List<MeasureGroup>();
            MeasureGroup group = null;
            measures.ForEach(mea =>
            {
                if (@group == null)
                    @group = new MeasureGroup();
                if (mea.ParentId > 1)
                {
                    if (@group.Parent == null || @group.Parent.ID != mea.ParentId)
                    {
                        if (@group.Measures != null && @group.Measures.Count > 0)
                        {
                            measureJson.Add(@group);
                            @group = new MeasureGroup();
                        }
                        @group.Parent = new MeasureWaveModel()
                        {
                            ID = mea.ParentId,
                            Name = mea.ParentMeasureName,
                            Waves = parentMeasures.Find(x => x.MeasureId == mea.ParentId).ApplyToWave
                        };
                        if (@group.Measures != null) @group.Measures.Clear();
                    }
                    if (@group.Measures == null)
                        @group.Measures = new List<MeasureWaveModel>();
                    // 屏蔽Total
                    if (mea.MeasureId != mea.ParentId)
                        @group.Measures.Add(
                            new MeasureWaveModel()
                            {
                                ID = mea.MeasureId,
                                Name = mea.Name,
                                TheOtherLanguageName = mea.TheOtherLanguageName,
                                Waves = mea.ApplyToWave
                            });
                }
                else
                {
                    if (@group.Parent != null)
                    {
                        if (@group.Measures != null && @group.Measures.Count > 0)
                        {
                            measureJson.Add(@group);
                            @group = new MeasureGroup();
                        }
                        @group.Parent = null;
                    }
                    if (@group.Measures == null)
                        @group.Measures = new List<MeasureWaveModel>();
                    @group.Measures.Add(
                        new MeasureWaveModel()
                        {
                            ID = mea.MeasureId,
                            Name = mea.Name,
                            TheOtherLanguageName = mea.TheOtherLanguageName,
                            Waves = mea.ApplyToWave
                        });
                }
            });
            if (group != null)
                measureJson.Add(@group);
            return measureJson;
        }

        public static Dictionary<string, object> GetGroupJson(List<MeasureHeaderModel> measures, List<MeasureHeaderModel> parentMeasures)
        {
            var measureJson = MeasureGroup.Parse(measures, parentMeasures);
            var groups = new Dictionary<string, object>();
            groups.Add("groups", measureJson);
            var waves = new Dictionary<int, object>();
            var ws = Wave.BOY.ToList();
            foreach (var wave in ws)
            {
                var value = Convert.ToInt32(wave);

                waves.Add(value, new
                {
                    id = value,
                    text = "Wave " + wave.ToDescription(),
                    count =
                        measures.Count(x => !string.IsNullOrEmpty(x.ApplyToWave) && x.ApplyToWave.Contains(value.ToString()))
                });
            }
            groups.Add("waves", waves);
            return groups;
        }

        [JsonProperty("parent")]
        public MeasureWaveModel Parent { get; set; }

        [JsonProperty("measures")]
        public List<MeasureWaveModel> Measures { get; set; }
    }
}