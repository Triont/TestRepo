using System;
using System.Collections.Generic;
using System.Linq;
using Project2.Models;
using System.Threading.Tasks;

namespace Project2.Services
{
    public class CalculateService
    {

        public List<(double,DateTime)> GetSteps(List<(double, DateTime)> args, int Count)
        {
            int skip = 0;
        
            List<(double, DateTime)> temp = new List<(double, DateTime)>();
         
            for (int i = 0; i < args.Count;)
            {

                temp.Add((args[i].Item1, args[i].Item2));

                if (args.Count - i > Count)
                {
                    i += Count;
                }
                else
                {
                    i += (args.Count - i);
                }


            }
            return temp;

            
        }
        public List<(double, DateTime)> GetSteps(List<ModelData> args, int Count)
        {
            int skip = 0;
          
            List<(double, DateTime)> temp = new List<(double, DateTime)>();
           for( int i=0;i<args.Count;)
            {
               
                    temp.Add((args[i].Value, args[i].DateTime));
                
                if(args.Count-i>Count)
                {
                    i += Count;
                }
                else
                {
                    i += (args.Count - i);
                }
               

            }

           
            return temp;


        }
        public List<(double, DateTime)> Intersect(List<(double, DateTime)> argsFirst, List<(double, DateTime)> argsSecond, List<ModelData> models)
        {

            List<ValueTuple<double, DateTime>> ps = new List<(double, DateTime)>();
            for(int i=0;i<models.Count;i++)
            {
                ps.Add((models[i].Value, models[i].DateTime));
            }
            List<ValueTuple<double, DateTime>> ps1 = new List<(double, DateTime)>();
            for (int i = 0; i < argsFirst.Count; i++)
            {
                ps1.Add((argsFirst[i].Item1, argsFirst[i].Item2));
            }
            List<ValueTuple<double, DateTime>> ps2 = new List<(double, DateTime)>();
            for (int i = 0; i < argsSecond.Count; i++)
            {
                ps2.Add((argsSecond[i].Item1, argsSecond[i].Item2));
            }
            var temp_res = ps1.Intersect(ps2).ToList();
            var result = temp_res.Intersect(ps).ToList();
            if(result.Count==0)
            {
                return temp_res;
            }
            else
            {
                return result;
            }

        }

        public List<List<(double, DateTime)>> MaxMinFound(List<(double, DateTime)> args)
        {
            List<(double, DateTime)> _maxes = new List<(double, DateTime)>();
            List<(double, DateTime)> _mins = new List<(double, DateTime)>();
            for (int i = 1; i < args.Count - 1; i++)
            {

                if ((args[i].Item1 > args[i - 1].Item1) && (args[i].Item1 > args[i + 1].Item1))
                {
                    _maxes.Add((args[i].Item1, args[i].Item2));
                }
                if((args[i].Item1<args[i-1].Item1) && (args[i].Item1<args[i+1].Item1))
                {
                    _mins.Add((args[i].Item1, args[i].Item2));
                }
            }
            List<List<(double, DateTime)>> result = new List<List<(double, DateTime)>>();
            result.Add(_mins);
            result.Add(_maxes);
            return result;
        }

        public List<ModelData> TupleToModelData(List<(double, DateTime)> ps)
        {
            List<ModelData> res = new List<ModelData>();
            for(int i=0;i<ps.Count;i++)
            {
                ModelData modelData = new ModelData();
                modelData.DateTime = ps[i].Item2;
                modelData.Value = ps[i].Item1;
                res.Add(modelData);
            }
            return res;
        }

        public List<(double, DateTime)> GetAllPoints((double, DateTime)first, (double, DateTime) second)
        {
            List<(double, DateTime)> result = new List<(double, DateTime)>();
            var temp =new DateTime( (second.Item2.Year - first.Item2.Year), first.Item2.Month, first.Item2.Day);
            if(temp.Year>0)
            {
               var distance= Math.Sqrt(Math.Pow((second.Item1 - first.Item1), 2) + Math.Pow((second.Item2.Year - first.Item2.Year), 2));
                for(int i=1;i<=temp.Year;i++)
                {
                    (double, DateTime) tempPoint = new();
                    double y_t = ((second.Item1 - first.Item1) / temp.Year) * Math.Abs(i - first.Item1);
                    tempPoint.Item1 = y_t;
                    tempPoint.Item2 = new DateTime(first.Item2.Year + i, first.Item2.Month, first.Item2.Day, first.Item2.Hour, first.Item2.Minute, first.Item2.Second);
                    result.Add(tempPoint);
                }

            }
            result.Insert(0, first);
            result.Add(second);
            var NRes = result.Distinct().ToList();
            return NRes;
            
        
        }

    }
    
}
