using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class CategoriaAlimentoDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarCategoriaDeAlimento (CategoriaAlimento categoria)
        {
            try
            {
                if(categoria != null)
                {
                    db.CategoriaAlimentos.Add(categoria);
                    return await db.SaveChangesAsync();
                }

                return 0;//El objeto categoria viene vacio
            }
            catch (Exception)
            {

                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarCategoriaAlimento(int id)
        {
            try
            {
                if (id > 0)
                {
                    CategoriaAlimento cat = await db.CategoriaAlimentos.FindAsync(id);
                    if(cat!= null)
                    {
                        db.CategoriaAlimentos.Remove(cat);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El id es invalido
            }
            catch (Exception)
            {

                throw;
            }
        }

        //editar
        public async Task<int>EditarCategoriaAlimento(CategoriaAlimento categoria)
        {
            try
            {
                if (categoria != null)
                {
                    CategoriaAlimento categoriaExistente = await db.CategoriaAlimentos.FindAsync(categoria.IdCategoriaAlimento);
                    if(categoriaExistente != null)
                    {
                        categoriaExistente.NombreCategoria = categoria.NombreCategoria;
                        categoriaExistente.Descripcion = categoria.Descripcion;
                        categoriaExistente.Estado = categoria.Estado;

                        db.Entry(categoriaExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeo categoria viene vacio
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar todo
        public async Task<List<CategoriaAlimento>> ListarCategoriaAlimento()
        {
            try
            {
                return await db.CategoriaAlimentos.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por nombre
        public async Task<List<CategoriaAlimento>> ListarCategoriaAlimentoPorNombre(string nombre)
        {
            try
            {
                return await db.CategoriaAlimentos.Where(x => x.NombreCategoria == nombre && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por id y nombre
        public async Task<List<CategoriaAlimento>> ListarCategoriaAlimentoPorNombre(int id, string nombre)
        {
            try
            {
                return await db.CategoriaAlimentos.Where(x =>x.IdCategoriaAlimento != id && x.NombreCategoria == nombre && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por estado
        public async Task<List<CategoriaAlimento>> ListarCategoriaAlimentoPorEstado(int estado)
        {
            try
            {
                return await db.CategoriaAlimentos.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //buscar por id
        public async Task<CategoriaAlimento> BuscarCategoriaAlimentoPorId(int id)
        {
            try
            {
                if(id > 0)
                {
                    return await db.CategoriaAlimentos.FindAsync(id);
                }
                return null;//El id es invalido
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
