using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class AlimentoDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int> AgregarAlimento(Alimento alimento)
        {
            try
            {
                if(alimento != null)
                {
                    db.Alimentos.Add(alimento);
                    return await db.SaveChangesAsync();
                }
                return 0;//el objeto se recibe vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarAlimento(int id)
        {
            try
            {
                if(id > 0)
                {
                    Alimento alimento = await db.Alimentos.FindAsync(id);
                    if(alimento != null)
                    {
                        db.Alimentos.Remove(alimento);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//el id no es valido
            }
            catch (Exception)
            {

                throw;
            }
        }

        //editar
        public async Task<int>EditarAlimento(Alimento alimento)
        {
            try
            {
                if(alimento != null)
                {
                    Alimento alimentoExistente = await db.Alimentos.FindAsync(alimento.IdAlimento);
                    if(alimentoExistente != null)
                    {
                        alimentoExistente.Nombre = alimento.Nombre;
                        alimentoExistente.Descripcion = alimento.Descripcion;
                        alimentoExistente.Precio = alimento.Precio;
                        alimentoExistente.Imagen = alimento.Imagen;
                        alimentoExistente.Estado = alimento.Estado;
                        alimentoExistente.IdProveedor = alimento.IdProveedor;
                        alimentoExistente.IdCategoriaAlimento = alimento.IdCategoriaAlimento;

                        db.Entry(alimentoExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;//El objeto alimento viene vacio
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar todo
        public async Task<List<Alimento>> ListarAlimentos()
        {
            try
            {
                return await db.Alimentos.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por nombre
        public async Task<List<Alimento>> ListarAlimentosPorNombre(string nombre)
        {
            try
            {
                return await db.Alimentos.Where(x => x.Nombre == nombre).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por nombre
        public async Task<List<Alimento>> ListarAlimentosPorNombre(int id, string nombre)
        {
            try
            {
                return await db.Alimentos.Where(x =>x.IdAlimento != id && x.Nombre == nombre).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por estado
        public async Task<List<Alimento>> ListarAlimentosPorEstado(int estado)
        {
            try
            {
                return await db.Alimentos.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar por categoria
        public async Task<List<Alimento>> ListarAlimentosPorCategoria(int catergoria)
        {
            try
            {
                return await db.Alimentos.Where(x => x.IdCategoriaAlimento == catergoria && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Alimento> BuscarAlimentoPorId(int id)
        {
            try
            {
                if(id > 0)
                {
                    return await db.Alimentos.FindAsync(id);
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
