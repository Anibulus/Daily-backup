using System;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using static System.Console;

namespace Consola.Modelo
{
    public class CopyHelper
    {
        public List<Archivo> archivos { get; set; }
        public CopyHelper(int respaldosRealizados, int limiteDeRespaldos, string destino)
        {
            this.respaldosRealizados = respaldosRealizados;
            this.limiteDeRespaldos = limiteDeRespaldos;
            this.destino = destino;

        }
        public int respaldosRealizados { get; set; } = 0;
        public int limiteDeRespaldos { get; set; } = 5;
        public string destino { get; set; } = @"";

        public CopyHelper(string destino)
        {
            this.destino = destino;
            archivos = new List<Archivo>();
        }

        public void Respaldar()
        {
            if (archivos != null || archivos.Count > 0)
            {
                CrearDirectorioSiNoExiste();
                int tasaDeExito = archivos.Count;
                foreach (Archivo archivo in archivos)
                {
                    string actual = archivo.ubicacionActual + archivo.nombre + archivo.extension;
                    string destino = this.destino + archivo.nombre + archivo.extension;
                    try
                    {
                        if (!archivo.extension.Equals(""))
                            File.Copy(actual, destino, true);
                        else
                            DirectoryCopy(actual, destino);
                    }
                    catch (IOException iox)
                    {
                        tasaDeExito--;
                        Console.WriteLine(iox.Message);
                    }
                }
                WriteLine($"Se han respaldado {tasaDeExito} de {archivos.Count}. Tasa de éxito del {tasaDeExito * 100 / archivos.Count}%");
                this.respaldosRealizados++;
            }
            else
                WriteLine("El listado de archivos a respaldar debe contener al menos un archivo.");
        }//Fin de respaldar

        public void CrearDirectorioSiNoExiste()
        {
            if (!Directory.Exists(this.destino))
            {
                WriteLine("Se ha creado la carpeta destino");
                Directory.CreateDirectory(this.destino);
            }
        }

        public void Plazo(string plazo)
        {
            Timer t = new Timer
            {
                Interval = 2000
            };
            t.Enabled = true;
            //t.Tick+= new System.EventHandler(OnEven);
        }

        public void LimiteParaRespaldar()
        {
            if (this.respaldosRealizados >= this.limiteDeRespaldos)
            {
                this.respaldosRealizados--;
                //TODO eliminar respaldos
                Directory.Delete(this.destino);
            }
        }

        private static void DirectoryCopy(string actual, string destino)
        {
            // Obtiene las subcarpetas de la carpeta elegida
            DirectoryInfo dir = new DirectoryInfo(actual);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destino, file.Name);

                if (!Directory.Exists(destino))
                    Directory.CreateDirectory(destino);

                file.CopyTo(tempPath, true);
            }

            //Copia los subdirectorios
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destino, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }

        }//Fin de copy Directory

    }//Fin de clase
}//Fin de namespace