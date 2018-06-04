﻿using ErrorHandlingExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingExample.Services
{
    public class PrimalNumberCalculator : IPrimalNumberCalculator
    {
        public DivisionResult GetDividedNumbers(int number)
        {
            DivisionResult divisionResult = new DivisionResult();

            divisionResult.DividedNumber = number;
            divisionResult.DividingNumbers = new List<int>();

            for (int i = 0; i < (number / 2) + 1; i++)
            {
                if (number % i == 0)
                    divisionResult.DividingNumbers.Add(i);
            }

            return divisionResult;
        }
    }
}