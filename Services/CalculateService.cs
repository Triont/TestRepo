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
            while(args.Count-skip!=0)
            {
                var s = args.Skip(skip).Take(Count).ToList();
                temp.Add(s[0]);
                temp.Add(s[s.Count - 1]);
                if(args.Count-skip<Count)
                {
                    skip += (args.Count - skip);
                }
                else
                {
                    skip += Count;
                }
            }
            return temp;

            
        }
        public List<(double, DateTime)> GetSteps(List<ModelData> args, int Count)
        {
            int skip = 0;
          
            List<(double, DateTime)> temp = new List<(double, DateTime)>();
            while (args.Count - skip != 0)
            {
                var s = args.Skip(skip).Take(Count).ToList();
                temp.Add((s.FirstOrDefault().Value, s.FirstOrDefault().DateTime));
                temp.Add((s.LastOrDefault().Value, s.LastOrDefault().DateTime));
                if (args.Count - skip < Count)
                {
                    skip += (args.Count - skip);
                }
                else
                {
                    skip += Count;
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


    }
}
