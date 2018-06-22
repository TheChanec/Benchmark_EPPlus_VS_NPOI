﻿using Library_benchmark.Models;
using System;
using System.Collections.Generic;

namespace Library_benchmark.Helpers
{
    public class Consultas
    {
        private int rows;

        /// <summary>
        /// Contructor Inicial de la clase consultas 
        /// </summary>
        /// <param name="rows"> Numero de registros que se retornaran en un List<Dummy> </param>
        public Consultas(int rows)
        {
            this.rows = rows;
        }

        /// <summary>
        /// Obtiene el la lista generada de entidad Dummy
        /// </summary>
        /// <returns></returns>
        internal IList<Dummy> GetInformacion()
        {
            if (rows > 0)
            {
                IList<Dummy> respuesta = new List<Dummy>();
                for (int i = 1; i <= rows; i++)
                {
                    respuesta.Add(new Dummy
                    {
                        Fecha1 = DateTime.Now.Date,
                        Fecha2 = DateTime.Now.Date,
                        Fecha3 = DateTime.Now.Date,
                        Fecha4 = DateTime.Now.Date,
                        Fecha5 = DateTime.Now.Date,
                        Fecha6 = DateTime.Now.Date,

                        Moneda1 = (decimal)i,
                        Moneda2 = (decimal)i,
                        Moneda3 = (decimal)i,
                        Moneda4 = (decimal)i,
                        Moneda5 = (decimal)i,
                        Moneda6 = (decimal)i,

                        Propiedad13 = "row  " + i,
                        Propiedad14 = "row  " + i,
                        Propiedad15 = "row  " + i,
                        Propiedad16 = "row  " + i,
                        Propiedad17 = "row  " + i,
                        Propiedad18 = "row  " + i,
                        Propiedad19 = "row  " + i,
                        Propiedad20 = "row  " + i,
                        Propiedad21 = "row  " + i,
                        Propiedad22 = "row  " + i,
                        Propiedad23 = "row  " + i,
                        Propiedad24 = "row  " + i,
                        Propiedad25 = "row  " + i,
                        Propiedad26 = "row  " + i

                    });
                }
                return respuesta;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}