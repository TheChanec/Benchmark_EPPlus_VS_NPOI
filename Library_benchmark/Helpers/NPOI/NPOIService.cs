﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Library_benchmark.Models;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Library_benchmark.Helpers
{
    public class NPOIService
    {
        private IList<Dummy> informacion;
        private bool design;
        private XSSFWorkbook excel;
        private ICellStyle dateStyle;
        private XSSFSheet currentsheet;
        private XSSFSheet basesheet;
        private int rowInicial;


        /// <summary>
        /// Contructor base para NPOI
        /// </summary>
        /// <param name="informacion">Lista de registros que se incrustaran en las hojas</param>
        /// <param name="design">parametro bandera para definir si se pondra diseño a las hojas</param>
        /// <param name="sheets">numero de hojas que tendra el workbook</param>
        public NPOIService(IList<Dummy> informacion, bool design, int sheets)
        {
            this.informacion = informacion;
            this.design = design;
            if (design)
                this.rowInicial = 4;
            else
                this.rowInicial = 1;

            createWorkBook();
            createSheets(sheets);

        }

        /// <summary>
        /// Contructor para armar el NPOI en base a un template ya establecido
        /// </summary>
        /// <param name="excelFile">archivo template en arreglo de bytes</param>
        /// <param name="informacion">informacion que se incrustara en las hojas</param>
        /// <param name="sheets">numero de hojas que tendra el workbook</param>
        public NPOIService(byte[] excelFile, IList<Dummy> informacion, int sheets)
        {
            this.informacion = informacion;
            this.design = false;
            this.rowInicial = 4;

            createWorkBook(excelFile);
            createSheetBase();
            createSheets(sheets);
        }


        /// <summary>
        /// obtiene la primera hoja de el template para utilizarla como hoja base para armar el workbook
        /// </summary>
        private void createSheetBase()
        {
            basesheet = (XSSFSheet)excel.GetSheetAt(0);
        }

        /// <summary>
        /// crea un workbook base 
        /// </summary>
        private void createWorkBook()
        {
            excel = new XSSFWorkbook();

        }

        /// <summary>
        /// crea un workbook en base al templete establecido 
        /// </summary>
        /// <param name="excelFile">Templete</param>
        private void createWorkBook(byte[] excelFile)
        {

            var fs = new MemoryStream(excelFile);
            //excel = new HSSFWorkbook(fs);
            excel = (XSSFWorkbook)WorkbookFactory.Create(fs);
        }

        /// <summary>
        /// Funcion que crea hojas en el workbook
        /// </summary>
        /// <param name="sheets">numero de hojas que se crearan</param>
        private void createSheets(int sheets)
        {
            for (int i = 0; i < sheets; i++)
            {
                addSheet("Sheet" + i);
                addcabeceras();

                addInformation();

                PutFitInCells();
            }
        }

        /// <summary>
        /// Pone un autoFit en las columnas
        /// </summary>
        private void PutFitInCells()
        {


            int noOfColumns = currentsheet.GetRow(rowInicial - 1).LastCellNum;
            for (int j = 0; j < noOfColumns; j++)
            {
                currentsheet.AutoSizeColumn(j, false);
            }





        }

        /// <summary>
        /// Se encarga de poner el titulo de las tablas y de definil el estilo que tendra cada columna por defecto
        /// </summary>
        private void addcabeceras()
        {
            IRow row;
            row = currentsheet.GetRow(rowInicial - 1);
            if (row == null)
            {
                row = currentsheet.CreateRow(rowInicial - 1);
            }

            int cell = 0;

            var item = informacion.FirstOrDefault();



            foreach (var prop in item.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                var celda = row.GetCell(cell);
                if (celda == null)
                {
                    celda = row.CreateCell(cell);
                }

                if (design)
                {
                    var hfont = excel.CreateFont();
                    hfont.FontHeightInPoints = 12;
                    hfont.Color = IndexedColors.Black.Index;
                    hfont.FontName = "Century Gothic";

                    if (prop.PropertyType.Equals(typeof(DateTime)))
                    {
                        var style = excel.CreateCellStyle();
                        style.DataFormat = excel.CreateDataFormat().GetFormat("m/d/yyyy");
                        
                        style.SetFont(hfont);
                        currentsheet.SetDefaultColumnStyle(cell, style);

                    }
                    else if (prop.PropertyType.Equals(typeof(decimal)))
                    {
                        var style = excel.CreateCellStyle();
                        style.DataFormat = excel.CreateDataFormat().GetFormat("[$$-409]#,##0.00");
                        
                        style.SetFont(hfont);
                        currentsheet.SetDefaultColumnStyle(cell, style);
                        celda.SetCellType(CellType.Numeric);
                    }
                    else
                    {
                        var style = excel.CreateCellStyle();
                        
                        style.SetFont(hfont);
                        currentsheet.SetDefaultColumnStyle(cell, style);
                        celda.SetCellType(CellType.Numeric);
                    }
                }



                cell++;
                celda.SetCellValue(prop.Name.ToString());
            }
        }

        /// <summary>
        /// Agrega Sheet a el excel en base a nombre
        /// </summary>
        /// <param name="name"></param>
        private void addSheet(string name)
        {
            currentsheet = (XSSFSheet)excel.GetSheet(name);
            if (currentsheet == null)
            {
                if (basesheet != null)
                    currentsheet = (XSSFSheet)basesheet.CopySheet(name, true);
                else
                    currentsheet = (XSSFSheet)excel.CreateSheet(name);

            }
            currentsheet.DefaultRowHeight = 300;
        }

        /// <summary>
        /// Agrega informacion a la Sheet que este en memoria 
        /// </summary>
        private void addInformation()
        {
            int cont = rowInicial;
            foreach (var item in informacion)
            {
                IRow row;
                row = currentsheet.GetRow(cont);
                if (row == null)
                    row = currentsheet.CreateRow(cont);

                int cell = 0;

                foreach (var prop in item.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
                {
                    ICell celda;
                    celda = row.GetCell(cell);
                    if (celda == null)
                        celda = row.CreateCell(cell);

                    var style = currentsheet.GetColumnStyle(cell);
                    if (prop.PropertyType.Equals(typeof(DateTime)))
                    {
                        var date = (DateTime)prop.GetValue(item, null);
                        celda.SetCellValue(date.Date);
                        style.DataFormat = excel.CreateDataFormat().GetFormat("MM/dd/yyyy");



                    }
                    else if (prop.PropertyType.Equals(typeof(decimal)))
                    {
                        var money = (decimal)prop.GetValue(item, null);
                        celda.SetCellValue(Convert.ToDouble(money));
                        style.DataFormat = excel.CreateDataFormat().GetFormat("[$$-409]#,##0.00");

                        celda.SetCellType(CellType.Numeric);

                    }
                    else
                        celda.SetCellValue(prop.GetValue(item, null).ToString());


                    celda.CellStyle = style;
                    cell++;
                }
                cont++;
            }

        }

        /// <summary>
        /// Obtiene el dato del Workbook 
        /// </summary>
        /// <returns>dato referente a el excel que se esta armando</returns>
        internal XSSFWorkbook GetExcelExample()
        {
            return excel;
        }

    }
}