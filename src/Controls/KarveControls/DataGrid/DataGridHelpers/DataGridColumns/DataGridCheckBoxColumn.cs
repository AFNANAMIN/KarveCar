﻿
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Telerik.WinControls.UI;
using System.Drawing;

public class DataGridCheckBoxColumn : GridViewCheckBoxColumn
{

    int _Item;
    string _Campo;
    string _Tabla;
    string _ExpresionBd;

    System.Drawing.Color _BackColor = System.Drawing.Color.White;
    public int Item
    {
        get { return _Item; }
        set { _Item = value; }
    }

    public string Tabla
    {
        get { return _Tabla; }
        set { _Tabla = value; }
    }

    public string Campo
    {
        get { return _Campo; }
        set { _Campo = value; }
    }

    public string AliasCampo
    {
        get { return FieldName; }
        set { FieldName = value; }
    }

    public string ExpresionBd
    {
        get { return _ExpresionBd; }
        set { _ExpresionBd = value; }
    }

    public System.Drawing.Color BackColor
    {
        get { return _BackColor; }
        set { _BackColor = value; }
    }

    public override bool ReadOnly
    {
        get { return base.ReadOnly; }
        set
        {
            base.ReadOnly = value;
            if (value)
            {
            //    _BackColor = ColorSel;
            }
        }
    }
}
