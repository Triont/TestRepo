using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Project2.Models;
using System.Threading.Tasks;
using Project2.Services;
using Newtonsoft.Json;

namespace Project2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        private readonly ILogger<WeatherForecastController> _logger;
        public static List<ModelData> modelDatas { get; set; }
        public CalculateService CalculateService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CalculateService calculateService)
        {
            _logger = logger;
            CalculateService = calculateService;
        }

        [HttpGet]
        public ModelData[] []Get()
        {
            var rng = new Random();
            
            var _sDataVal = new List<(double, DateTime)>();
            List<(double, DateTime)> _tDataVal = new List<(double, DateTime)>();
            Dictionary<DateTime, double> Date_Value = new Dictionary<DateTime, double>();
            
            var s = ProcessingVector(ref Date_Value);
            var result = Diagram(s, ref Date_Value, out _tDataVal);

            List<ValueAndDate> first = new List<ValueAndDate>();

            DataToJson dataToJson = new DataToJson();


            var tmp = JsonConvert.SerializeObject(new { modelDatas, s, result });
            
            List<ValueAndDate> firstData = new List<ValueAndDate>();
            List<ValueAndDate> secondData = new List<ValueAndDate>();
            List<ValueAndDate> thirdData = new List<ValueAndDate>();

            for (int i = 0; i < modelDatas.Count; i++)
            {
                ValueAndDate tempValue = new ValueAndDate();
                tempValue.DateTime = modelDatas.Select(q => q.DateTime).ToList()[i];
                tempValue.Value = modelDatas.Select(q => q.Value).ToList()[i];
                first.Add(tempValue);
            }
            var getsTwo = ChangeData(s, ref Date_Value);
            var getsThird = ChangeData(result, ref Date_Value);

          var qq=  GetPs(getsTwo, getsThird);

            //Using Service
            var _modelToListService = CalculateService.GetSteps(modelDatas, 3);
          var serviceCalcService =  CalculateService.GetSteps(_modelToListService, 3);
            var intersectService = CalculateService.Intersect(_modelToListService, serviceCalcService, WeatherForecastController.modelDatas);
          var result_minmax=  CalculateService.MaxMinFound(intersectService);
            var service_mins = result_minmax[0];
            var service_maxs = result_minmax[1];

            var lst_s = new List<(double, DateTime)>();
            var lst_f = new List<(double, DateTime)>();
            for(int i=0;i<serviceCalcService.Count-1;i+=2)
            {
                lst_s.AddRange(CalculateService.GetAllPoints(serviceCalcService[i], serviceCalcService[i + 1]));
            }
            for (int i = 0; i < _modelToListService.Count - 1; i += 2)
            {
                lst_f.AddRange(CalculateService.GetAllPoints(_modelToListService[i], _modelToListService[i + 1]));
            }
            var newIntersect = CalculateService.Intersect(lst_f, lst_s, modelDatas);
            var newMinMax = CalculateService.MaxMinFound(newIntersect);
            var NewMins = newMinMax[0];
            var NewMaxs = newMinMax[1];

            //end of service using;



            var __f = TempDataValue();
           var __s = SecondApprox(__f);

            List<(DateTime, double)> firstCorrectFormat;
            List<(DateTime, double)> secondCorrectFormat;

            var _new_f = ChangeDataNew(__f, out firstCorrectFormat, 0);
            var _new_s = ChangeDataNew(__s, out secondCorrectFormat, 1);
            var _tmp = DiagrmInter(_new_f, _new_s);
            var _inters = DiagrmInter(firstCorrectFormat, secondCorrectFormat);
            List<(double, string, DateTime)> FindMinMax = new List<(double, string, DateTime)>();
          
         //   var maxmins = ResultCalcNew(_inters);

           var lastRes= ResultCalcNew(_tmp);
            var Mins = lastRes[0];
            var Maxes = lastRes[1];


            // return modelDatas.ToArray();
            //   return modelDatas.ToArray();

         
         var values=  _tmp.Select(i => i.Item1).ToList();
           var date= _tmp.Select(o => o.Item3).ToList();
            ModelData[] modelDatasN = new ModelData[values.Count];
            for(int i=0;i<modelDatasN.Length;i++)
            {
                modelDatasN[i] = new ModelData();
                modelDatasN[i].Value = values[i];
                modelDatasN[i].DateTime = date[i];
            }

            ModelData[] modelsArray = new ModelData[_inters.Count];
            for(int i=0; i<_inters.Count;i++)
            {
                modelsArray[i] = new ModelData();
                modelsArray[i].DateTime = _inters[i].Item1;
                modelsArray[i].Value = _inters[i].Item2;
            }
            //  return modelsArray;

            ModelData[] _ModelData = new ModelData[_new_s.Count];
            for(int i=0; i<_new_s.Count;i++)
            {
                _ModelData[i] = new ModelData();
                _ModelData[i].DateTime = _new_s[i].Item3;
                _ModelData[i].Value = _new_s[i].Item1;
            }


            ModelData[] SecondResModel = new ModelData[secondCorrectFormat.Count];
            for (int i = 0; i < secondCorrectFormat.Count; i++)
            {
                SecondResModel[i] = new ModelData();
                SecondResModel[i].DateTime = secondCorrectFormat[i].Item1;
                SecondResModel[i].Value = secondCorrectFormat[i].Item2;
            }


            //ModelData[][] arrView = new ModelData[2][];
            //arrView[0] = modelDatas.ToArray();
            //arrView[1] = modelsArray;
            // return SecondResModel;
            //    return arrView;

            ModelData[] ServiceIntersectModel = new ModelData[intersectService.Count];
            for(int i=0;i<intersectService.Count;i++)
            {
                ServiceIntersectModel[i] = new ModelData();
                ServiceIntersectModel[i].Value = intersectService[i].Item1;
                ServiceIntersectModel[i].DateTime = intersectService[i].Item2;
            }


         var s_mins=   CalculateService.TupleToModelData(service_mins).ToArray();
          var s_maxs=  CalculateService.TupleToModelData(service_maxs).ToArray();
            ModelData[][]arr_toShow= new ModelData[10][];
            arr_toShow[0] = ServiceIntersectModel;
            arr_toShow[1] = s_mins;
            arr_toShow[2] = s_maxs;
            arr_toShow[3] = modelDatas.ToArray();
            arr_toShow[4] = CalculateService.TupleToModelData(_modelToListService).ToArray();
            arr_toShow[5] = CalculateService.TupleToModelData(serviceCalcService).ToArray();
            arr_toShow[6] = CalculateService.TupleToModelData(newIntersect).ToArray();
            arr_toShow[7] = CalculateService.TupleToModelData(NewMins).ToArray();
            arr_toShow[8] = CalculateService.TupleToModelData(NewMaxs).ToArray();
         
        //     return ServiceIntersectModel;
          return arr_toShow;
            //return modelsArray;
          
            
          //  return modelDatas.ToArray();

        }
        public static List<ValueAndDate> ChangeData(List<(double, double, string)> ps, ref Dictionary<DateTime, double> keyValuePairs)
        {
            List<ValueAndDate> result = new List<ValueAndDate>();
            for(int i=0;i<ps.Count;i++)
            {
                ValueAndDate valueAndDate = new ValueAndDate();
                valueAndDate.Value = ps[i].Item1;
                valueAndDate.DateTime = keyValuePairs.Where(z => z.Value == ps[i].Item1).Select(p => p.Key).FirstOrDefault();
                ValueAndDate valueAndDate2 = new ValueAndDate();
                valueAndDate2.Value = ps[i].Item2;
                valueAndDate2.DateTime = keyValuePairs.Where(z => z.Value == ps[i].Item2).Select(p => p.Key).FirstOrDefault();
                if(!result.Contains(valueAndDate))
                {
                    result.Add(valueAndDate);
                }
                if(!result.Contains(valueAndDate2))
                {
                    result.Add(valueAndDate2);
                }

            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="outParams"></param>
        /// <param name="type">0 --for first List, 1--for second</param>
        /// <returns></returns>
        public static List<ValueTuple<double, string,DateTime>> ChangeDataNew(List<(double, double, string, DateTime)> ps, out List<(DateTime, double)> outParams, int type)
        {
            List<ValueTuple<double,string, DateTime>> result = new List<(double,string, DateTime)>();

            List<ValueTuple<DateTime, double>> temp_value = new List<(DateTime, double)>();

            switch(type)
            {
                case 0:
                    for (int i = 0; i < ps.Count; i++)
                    {
                        ValueTuple<DateTime, double> loopVarFirst = (DateTime.MinValue, 0);

                        loopVarFirst.Item1 = ps[i].Item4.AddYears(-3);//-2
                        loopVarFirst.Item2 = ps[i].Item1;
                        temp_value.Add(loopVarFirst);
                        ValueTuple<DateTime, double> loopVarSecond = (DateTime.MinValue, 0);
                        loopVarSecond.Item1 = ps[i].Item4;
                        loopVarSecond.Item2 = ps[i].Item2;

                        temp_value.Add(loopVarSecond);
                    }
                    break;
                case 1:
                    for (int i = 0; i < ps.Count; i++)
                    {
                        ValueTuple<DateTime, double> loopVarFirst = (DateTime.MinValue, 0);

                        loopVarFirst.Item1 = ps[i].Item4.AddYears(-9);//-6
                        loopVarFirst.Item2 = ps[i].Item1;
                        temp_value.Add(loopVarFirst);
                        ValueTuple<DateTime, double> loopVarSecond = (DateTime.MinValue, 0);
                        loopVarSecond.Item1 = ps[i].Item4;
                        loopVarSecond.Item2 = ps[i].Item2;

                        temp_value.Add(loopVarSecond);
                    }
                    break;
            }

          outParams= temp_value.Distinct().ToList();


            switch(type)
            {
                case 0:
                    for (int i = 0; i < ps.Count; i++)
                    {
                        //ValueAndDate valueAndDate = new ValueAndDate();
                        //valueAndDate.Value = ps[i].Item1;
                        //valueAndDate.DateTime = ps[i].Item4.AddYears(-9);

                        if (!result.Contains((ps[i].Item1, ps[i].Item3, ps[i].Item4.AddYears(-9))))
                        {
                            result.Add((ps[i].Item1, ps[i].Item3, ps[i].Item4.AddYears(-9)));
                        }

                        if (!result.Contains((ps[i].Item2, ps[i].Item3, ps[i].Item4)))
                        {
                            result.Add((ps[i].Item2, ps[i].Item3, ps[i].Item4));
                        }
                        //ValueAndDate valueAndDate2 = new ValueAndDate();
                        //valueAndDate2.Value = ps[i].Item2;
                        //valueAndDate2.DateTime = ps[i].Item4;
                        //if (!result.Contains(valueAndDate))
                        //{
                        //    result.Add(valueAndDate);
                        //}
                        //if (!result.Contains(valueAndDate2))
                        //{
                        //    result.Add(valueAndDate2);
                        //}

                    }
                    break;
                case 1:

                    for (int i = 0; i < ps.Count; i++)
                    {
                        //ValueAndDate valueAndDate = new ValueAndDate();
                        //valueAndDate.Value = ps[i].Item1;
                        //valueAndDate.DateTime = ps[i].Item4.AddYears(-9);

                        if (!result.Contains((ps[i].Item1, ps[i].Item3, ps[i].Item4.AddYears(-9))))//-6
                        {
                            result.Add((ps[i].Item1, ps[i].Item3, ps[i].Item4.AddYears(-9)));//-6
                        }

                        if (!result.Contains((ps[i].Item2, ps[i].Item3, ps[i].Item4)))
                        {
                            result.Add((ps[i].Item2, ps[i].Item3, ps[i].Item4));
                        }
                        //ValueAndDate valueAndDate2 = new ValueAndDate();
                        //valueAndDate2.Value = ps[i].Item2;
                        //valueAndDate2.DateTime = ps[i].Item4;
                        //if (!result.Contains(valueAndDate))
                        //{
                        //    result.Add(valueAndDate);
                        //}
                        //if (!result.Contains(valueAndDate2))
                        //{
                        //    result.Add(valueAndDate2);
                        //}

                    }
                    break;
            }
          
            return result.Distinct().ToList();
        }

        public static List<(double, double, string)> ProcessingVector( ref Dictionary<DateTime, double> ps)
        {
           
           // ps = new List<(double, DateTime)>();
            List<(double, double, string)> result = new List<(double, double, string)>();
            if ((modelDatas == null) || modelDatas.Count == 0)
            { Random rng = new Random();

                WeatherForecastController.modelDatas = new List<ModelData>();
                for (int i = 0; i < 1000; i++)
                {
                    WeatherForecastController.modelDatas.Add(new ModelData { DateTime = new DateTime(1950 + i, 2, 5, 1, 6, 9), Value = rng.NextDouble() + 1.5 });
                }
            }
            int Skip = 0;
           

            while ((modelDatas.Count - Skip) != 0)
            //if (modelDatas.Count - Skip >= 1)
            {



                var temp = WeatherForecastController.modelDatas.Skip(Skip).Take(3).ToList();

                if (temp[0]?.Value > temp.Last()?.Value)
                {
                    result.Add((temp[0].Value, temp.Last().Value, "decrease"));
                    if (!ps.ContainsKey(temp[0].DateTime))
                    {
                        ps.Add(temp[0].DateTime, temp[0].Value);
                    }

                    if (!ps.ContainsKey(temp[temp.Count - 1].DateTime))
                    {
                        ps.Add(temp[temp.Count - 1].DateTime, temp[temp.Count - 1].Value);
                    }
                }
                else if (temp[0]?.Value < temp.Last()?.Value)
                {
                    result.Add((temp[0].Value, temp.Last().Value, "increase"));

                    if (!ps.ContainsKey(temp[0].DateTime))
                    {
                        ps.Add(temp[0].DateTime, temp[0].Value);
                    }
                    if (!ps.ContainsKey(temp[temp.Count - 1].DateTime))
                    {
                        ps.Add(temp[temp.Count - 1].DateTime, temp[temp.Count - 1].Value);
                    }
                }
                else
                {
                    result.Add((temp[0].Value, temp.Last().Value, "equal"));
                    if (!ps.ContainsKey(temp[0].DateTime))
                    {
                        ps.Add(temp[0].DateTime, temp[0].Value);
                    }
                    if (!ps.ContainsKey(temp[temp.Count - 1].DateTime))
                    {

                        ps.Add(temp[temp.Count - 1].DateTime, temp[temp.Count - 1].Value);
                    }
                }
                if (modelDatas.Count - Skip < 3)
                {
                    Skip += modelDatas.Count - Skip;
                }
                else
                {


                    Skip += 3;
                }
            }
          

            return result;
        }
        public static List<(double, double, string)> Diagram(List<(double, double, string)> v,ref Dictionary<DateTime, double> DateTimeValue , out List<(double, DateTime)> ps)
        {
            var skip = 0;
            List<(double, double, string)> result = new List<(double, double, string)>();

            ps = new List<(double, DateTime)>();

            while ((modelDatas.Count - skip) != 0)
            {


                var new_temp = v.Skip(skip).Take(9).ToList();

                for (int i = 0; i < new_temp.Count; i++)
                {
                    
                    if (new_temp?[i].Item1 > new_temp?[i].Item2)
                    {
                        result.Add((new_temp[i].Item1, new_temp[i].Item2, "decrease"));

                     

                    }
                    else if (new_temp?[i].Item1 < new_temp?[i].Item2)
                    {
                        result.Add((new_temp[i].Item1, new_temp[i].Item2, "increase"));
                  
                    }
                    else
                    {
                        if (new_temp != null)
                        {


                            result.Add((new_temp[i].Item1, new_temp[i].Item2, "equal"));

                        }
                          
                    }
                }
                if (modelDatas.Count - skip < 9)
                {
                    skip += (modelDatas.Count - skip);
                }
                else
                {
                    skip += 9;
                }


            }
            return result;
        }


        public static List<ValueTuple<DateTime, double>> GetPs(List<ValueAndDate> f, List<ValueAndDate> s)
        {
            var inter = f.Intersect(s).ToList();

            List<ValueTuple<DateTime, double>> ps = new List<(DateTime, double)>();
            List<ValueTuple<DateTime, double>> second = new List<(DateTime, double)>();
            List<ValueTuple<DateTime, double>> third = new List<(DateTime, double)>();
            for(int i=0; i<f.Count;i++)
            {
                ValueTuple<DateTime, double> npd = new(DateTime.Now, 0);
                var _temp = f[i].Value;
                var _d = f[i].DateTime;
                npd.Item1 = _d;
                npd.Item2 = _temp;
                ps.Add(npd);

            }

            for (int i = 0; i < s.Count; i++)
            {
                ValueTuple<DateTime, double> npd = new(DateTime.Now, 0);
                var _temp = s[i].Value;
                var _d = s[i].DateTime;
                npd.Item1 = _d;
                npd.Item2 = _temp;
                second.Add(npd);

            }
            var _result = ps.Intersect(second).ToList();

            List<ValueAndDate> valueAndDates = new List<ValueAndDate>();
            for (int i = 0; i < modelDatas.Count; i++)
            {
              
                ValueTuple<DateTime, double> __temp = new(DateTime.Now, 0);
                __temp.Item1 = modelDatas[i].DateTime;
                __temp.Item2 = modelDatas[i].Value;
                third.Add(__temp);
            }
        var new_result=    _result.Intersect(third).ToList();
             //Intersect(valueAndDates).ToList();
            return _result;
        }

       
        public static List<ValueTuple<double, string,DateTime>> DiagrmInter(List<ValueTuple<double,string, DateTime>>  f, List<ValueTuple<double,string, DateTime>> s)
        {
           
          var s_th=  f.Intersect(s).ToList();
            List<ValueTuple<double,string, DateTime>> temp = new List<ValueTuple<double,string, DateTime>>();
            for(int i=0; i<modelDatas.Count;i++)
            {
                ValueTuple<double,string, DateTime> d;
                d.Item1 = modelDatas[i].Value;
                d.Item2 = f[i/3].Item2;
                d.Item3 = modelDatas[i].DateTime;
                temp.Add(d);

            }
            var _r_f = f.Intersect(temp).ToList();
            var _r_s = s.Intersect(temp).ToList();
          var result=  s_th.Intersect(temp).ToList();
            return result;
        }



        public static List<ValueTuple<DateTime, double>> DiagrmInter(List<ValueTuple<DateTime, double>> f, List<ValueTuple<DateTime, double>> s)
        {

            var s_th = f.Intersect(s).ToList();
            List<ValueTuple<DateTime, double>> temp = new List<ValueTuple<DateTime,  double>>();
            for (int i = 0; i < modelDatas.Count; i++)
            {
                ValueTuple<DateTime, double> d;
                d.Item1 = modelDatas[i].DateTime;
            //    d.Item2 = f[i / 3].Item2;
                d.Item2 = modelDatas[i].Value;
                temp.Add(d);

            }
            var _r_f = f.Intersect(temp).ToList();
            var _r_s = s.Intersect(temp).ToList();
            var result = s_th.Intersect(temp).ToList();
            if(result.Count==0)
            {
                return s_th;
            }
            return result;
        }

        public List<(double, DateTime)> FinalResult(List<(double, DateTime)> points)
        {
            List<(double, DateTime)> maxes = new List<(double, DateTime)>();
            List<(double, DateTime)> mins = new List<(double, DateTime)>();
           

            var skip = 0;
            //start and end list
            List<(double, double,  string)> result = new List<(double, double, string)>();
            List<(List<(double, DateTime)>, List<(double, DateTime)>)> newnew = new List<(List<(double, DateTime)>, List<(double, DateTime)>)>();
            var ps = new List<(double, DateTime)>();
            while ((modelDatas.Count - skip) != 0)
            {




                var temp = points.Skip(skip).Take(9).ToList();
                if (temp[0].Item1 > temp[temp.Count-1].Item1)
                {
                    result.Add((temp[0].Item1, temp.Last().Item1, "decrease"));
                  

                }
                else if (temp?[0].Item1 < temp?[temp.Count-1].Item1)
                {
                    result.Add((temp[0].Item1, temp.Last().Item1, "increase"));
                    //ps.Add((temp[0].Value, temp[0].DateTime));
                    //ps.Add((temp[temp.Count - 1].Value, temp[temp.Count - 1].DateTime));
                }
                else
                {
                    if (temp != null)
                    {


                        result.Add((temp[0].Item1, temp.Last().Item1, "equal"));

                    }
                      
                }

                if (modelDatas.Count - skip < 9)
                {
                    skip += (modelDatas.Count - skip);
                }
                else
                {
                    skip += 9;
                }

            }

            for(int i=1; i<result.Count;i++)
            {
                if(result[i].Item3 != result[i-1].Item3 && result[i].Item3!=result[i+1].Item3)
                {
                    if(result[i].Item1 > result[i-1].Item1)
                    {
                       // maxes.Add((result[i].Item1, dateTime.Where(t=>t.Value==result[i].Item1).Select(p=>p.Key).FirstOrDefault()));
                    }
                  
                }
            }



            return new List<(double, DateTime)>();
        }

     public List<List<(double, DateTime)>>ResultCalc(List<(double, double, string, DateTime)> arg)
        {
            List<(double, DateTime)> maxes = new List<(double, DateTime)>();
            List<(double, DateTime)> mins = new List<(double, DateTime)>();
            (double, double, string, DateTime) _tempTuple = (0, 0, "", DateTime.MinValue);
            
            for(int i=1; i<arg.Count;i++)
            {
                if(_tempTuple.Item1!=default(double) && _tempTuple.Item2!=default(double))
                {
                    if((_tempTuple.Item3!=arg[i].Item3)&& arg[i].Item3!="equal")
                    {
                        if(_tempTuple.Item3=="decrease")
                        {
                            mins.Add((_tempTuple.Item2, _tempTuple.Item4));
                        }
                        else if(_tempTuple.Item3=="increase")
                        {
                            maxes.Add((_tempTuple.Item2, _tempTuple.Item4));
                        }
                    }
                }
                if((arg[i].Item3!=arg[i-1].Item3)&& arg[i].Item3!="equal")
                {
                  
                    if(arg[i-1].Item3=="decrease")
                    {
                        mins.Add((arg[i - 1].Item2, arg[i - 1].Item4));
                    }
                    else if(arg[i-1].Item3=="increase")
                    {
                        maxes.Add((arg[i - 1].Item2, arg[i - 1].Item4));
                    }
                }
                else if((arg[i].Item3==arg[i-1].Item3)|| arg[i].Item3=="equal")
                {
                    _tempTuple.Item1 = arg[i - 1].Item1;
                    _tempTuple.Item2 = arg[i].Item2;
                    _tempTuple.Item3 = arg[i - 1].Item3;
                    _tempTuple.Item4 = arg[i].Item4;
                }
            }
            var _result = new List<List<(double, DateTime)>>();
            _result.Add(mins);
            _result.Add(maxes);
            return _result;
        }


        public List<List<(double, DateTime)>> ResultCalcNew(List<(double, string, DateTime)> arg)
        {
            List<(double, DateTime)> maxes = new List<(double, DateTime)>();
            List<(double, DateTime)> mins = new List<(double, DateTime)>();
            (double, string, DateTime, double, DateTime) _tempTuple = (0, "", DateTime.MinValue, 0, DateTime.MinValue);

            for (int i = 1; i < arg.Count; i++)
            {
                if (_tempTuple.Item1 != default(double))
                {
                    if ((_tempTuple.Item2 != arg[i].Item2) && arg[i].Item2 != "equal")
                    {
                        if (_tempTuple.Item2 == "decrease")
                        {
                            mins.Add((_tempTuple.Item1, _tempTuple.Item3));
                        }
                        else if (_tempTuple.Item2 == "increase")
                        {
                            maxes.Add((_tempTuple.Item1, _tempTuple.Item3));
                        }
                    }
                }
                if ((arg[i].Item2 != arg[i - 1].Item2) && arg[i].Item2 != "equal")
                {

                    if (arg[i - 1].Item2 == "decrease")
                    {
                        mins.Add((arg[i - 1].Item1, arg[i - 1].Item3));
                    }
                    else if (arg[i - 1].Item2 == "increase")
                    {
                        maxes.Add((arg[i - 1].Item1, arg[i - 1].Item3));
                    }
                }
                else if ((arg[i].Item2 == arg[i - 1].Item2) || arg[i].Item2 == "equal")
                {
                    _tempTuple.Item1 = arg[i - 1].Item1;
                    _tempTuple.Item4 = arg[i].Item1;
                    _tempTuple.Item3 = arg[i - 1].Item3;
                    _tempTuple.Item5 = arg[i].Item3;
                }
            }
            var _result = new List<List<(double, DateTime)>>();
            _result.Add(mins);
            _result.Add(maxes);
            return _result;
        }



        


        public static List<(double, double, string, DateTime)> TempDataValue()
        {
            Random rng = new Random();
            // ps = new List<(double, DateTime)>();
            List<(double, double, string, DateTime)> result = new List<(double, double, string, DateTime)>();
            WeatherForecastController.modelDatas = new List<ModelData>();
            for (int i = 0; i < 1000; i++)
            {
                WeatherForecastController.modelDatas.Add(new ModelData { DateTime = new DateTime(1950 + i, 2, 5, 1, 6, 9), Value = rng.NextDouble() + 1.5 });
            }
            int Skip = 0;
            //for (int i = 0; i < WeatherForecastController.modelDatas.Count; i++)
            //{

            while ((modelDatas.Count - Skip) != 0)
            //if (modelDatas.Count - Skip >= 1)
            {



                var temp = WeatherForecastController.modelDatas.Skip(Skip).Take(3).ToList();

                if (temp[0]?.Value > temp.Last()?.Value)
                {
                    result.Add((temp[0].Value, temp.Last().Value, "decrease", temp[temp.Count-1].DateTime));
                    //if (!ps.ContainsKey(temp[0].DateTime))
                    //{
                    //    ps.Add(temp[0].DateTime, temp[0].Value);
                    //}

                    //if (!ps.ContainsKey(temp[temp.Count - 1].DateTime))
                    //{
                    //    ps.Add(temp[temp.Count - 1].DateTime, temp[temp.Count - 1].Value);
                    //}
                }
                else if (temp[0]?.Value < temp.Last()?.Value)
                {
                    result.Add((temp[0].Value, temp.Last().Value, "increase", temp[temp.Count-1].DateTime));

                  
                }
                else
                {
                    result.Add((temp[0].Value, temp.Last().Value, "equal", temp[temp.Count-1].DateTime));
                
                }
                if (modelDatas.Count - Skip < 3)
                {
                    Skip += modelDatas.Count - Skip;
                }
                else
                {


                    Skip += 3;
                }
          
            }
            return result;
        }

        public static List<(double,double, string, DateTime)> SecondApprox(List<(double, double, string, DateTime)> args)
        {
            var skip = 0;
            List<(double, double, string, DateTime)> result = new List<(double start, double end, string vector, DateTime _dateTime)>();

        

            while ((args.Count - skip) != 0)
            {



                var temp = WeatherForecastController.modelDatas.Skip(skip).Take(3).ToList();
                var n_temp = args.Skip(skip).Take(3).ToList();
                if (n_temp?.Count != 0)
                {
                    if (n_temp?[0].Item1 > n_temp?[n_temp.Count - 1].Item2)
                    {
                        result.Add((n_temp[0].Item1, n_temp[n_temp.Count - 1].Item2, "decrease", n_temp[n_temp.Count - 1].Item4));
                      
                    }
                    else if (n_temp?[0].Item1 < n_temp?[n_temp.Count - 1].Item2)
                    {
                        // result.Add((temp[0].Value, temp.Last().Value, "increase", temp[temp.Count - 1].DateTime));
                        result.Add((n_temp[0].Item1, n_temp[n_temp.Count - 1].Item2, "increase", n_temp[n_temp.Count - 1].Item4));
                        
                        
                    }
                    else
                    {
                        if (n_temp != null)
                        {
                            result.Add((n_temp[0].Item1, n_temp[n_temp.Count - 1].Item2, "equal", n_temp[n_temp.Count - 1].Item4));
                        }
                       

                    }
                }
                if (args.Count - skip < 3)
                {
                    skip += args.Count - skip;
                }
                else
                {


                    skip += 3;
                }




            }
            return result;
        }

        public class DataToJson
        {
            public List<ValueAndDate> FirstChart { get; set; }
            public List<ValueAndDate> Second { get; set; }
            public List<ValueAndDate> Third { get; set; }
        }

        public class ValueAndDate
        {
            public double Value { get; set; }
            public DateTime DateTime { get; set; }
        }

    }
}
