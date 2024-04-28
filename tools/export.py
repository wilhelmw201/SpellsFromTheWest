# -*- coding: utf-8 -*-
"""
Created on Sat Mar 30 01:38:44 2024

@author: wilhelm
"""

import pandas as pd
import numpy as np
file_path = r'C:\Users\wilhelm\source\repos\SpellsFromTheWest\tools\spellsfromWest.xlsx'

datasheets = pd.read_excel(file_path, sheet_name=None)
all_calls = []
for sheet_name, datasheet in datasheets.items():
    datasheet.fillna("", inplace=True)
    function_start = f"DataConfigAppender.CreateAndAppend{sheet_name}FromStrings("
    
    for idx, row in datasheet.iterrows():
        if row.TemplateId < 4000:
            continue
        calls = [function_start]
        for field_name, field_val in row.items():

            calls.append(f'{field_name}:"{field_val}",')
        
        this_call = "".join(calls)[:-1]+");"
        all_calls.append(this_call)
    
for call in all_calls:
    print(call)
    
head = r"""
using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWest
{
    internal class AddConfig
    {

        public static void DoAppend()
        {
"""
tail = """
        }
    }
}
"""
with open(r'C:\Users\wilhelm\source\repos\SpellsFromTheWest\SpellsFromTheWestFrontend\AddConfig.cs', 
          "w",encoding='utf-8') as f:
    f.write(head)
    for call in all_calls:
        f.write(call)
        f.write("\n")
    f.write(tail)
    

with open(r'C:\Users\wilhelm\source\repos\SpellsFromTheWest\SpellsFromTheWest\AddConfig.cs', 
          "w",encoding='utf-8') as f:
    f.write(head)
    for call in all_calls:
        f.write(call)
        f.write("\n")
    f.write(tail)
    
