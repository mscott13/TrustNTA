using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class VacancyFilterData
    {
        public VacancyFilterData(List<EmployerVacancy> filteredVacancies, int startIndex, int limit) 
        {
            if (filteredVacancies != null) 
            {
                rawCount = filteredVacancies.Count;
                vacancies = new List<EmployerVacancy>();
                remaindingCount = filteredVacancies.Count - (startIndex + limit);

                if (remaindingCount < 0) 
                {
                    remaindingCount = 0;
                }
            }

            int upperlimit = startIndex + limit;
            for (int i = startIndex; i <= upperlimit; i++)
            {
                if (i == filteredVacancies.Count || vacancies.Count == limit)
                {
                    break;
                }

                filteredVacancies[i].index = i;
                vacancies.Add(filteredVacancies[i]);
            }
        }

        public List<EmployerVacancy> vacancies { get; set; }
        public int remaindingCount { get; }
        public int rawCount { get; }
    }
}

