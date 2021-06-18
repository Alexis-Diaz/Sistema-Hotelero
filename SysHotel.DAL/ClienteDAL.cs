using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using System.Data.Entity;

namespace SysHotel.DAL
{
    public class ClienteDAL
    {
        private BDComun db = new BDComun();

        //agregar
        public async Task<int>AgregarCliente(Cliente cliente)
        {
            try
            {
                if(cliente != null)
                {
                    db.Clientes.Add(cliente);
                    return await db.SaveChangesAsync();
                }
                return 0;// El objeto cliente viene vacio
            }
            catch (Exception)
            {

                throw;
            }
        }

        //eliminar
        public async Task<int>EliminarCliente(int id)
        {
            try
            {
                if (id > 0){
                    Cliente clienteExistente = await db.Clientes.FindAsync(id);
                    if(clienteExistente != null)
                    {
                        db.Clientes.Remove(clienteExistente);
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;// El id es invalido
            }
            catch (Exception)
            {

                throw;
            }
        }

        //editar
        public async Task<int>EditarCliente(Cliente cliente)
        {
            try
            {
                if(cliente!= null)
                {
                    Cliente clienteExistente = await db.Clientes.FindAsync(cliente.IdCliente);
                    if(clienteExistente != null)
                    {
                        clienteExistente.Nombres = cliente.Nombres;
                        clienteExistente.Apellidos = cliente.Apellidos;
                        clienteExistente.FechaNacimiento = cliente.FechaNacimiento;
                        clienteExistente.TipoDocumento = cliente.TipoDocumento;
                        clienteExistente.NumeroDocumento = cliente.NumeroDocumento;
                        clienteExistente.Telefono = cliente.Telefono;
                        clienteExistente.Correo = cliente.Correo;
                        clienteExistente.Direccion = cliente.Direccion;
                        clienteExistente.Estado = cliente.Estado;

                        db.Entry(clienteExistente).State = EntityState.Modified;
                        return await db.SaveChangesAsync();
                    }
                }
                return 0;// El objeto cliente viene vacio
            }
            catch (Exception)
            {
                throw;
            }
        }

        //listar todo
        public async Task<List<Cliente>> ListarClientes()
        {
            try
            {
                return await db.Clientes.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por numero documento, nombres, apellido y estado
        public async Task<List<Cliente>> ListarClienteUnico(string numeroDocumento, string nombre, string apellido)
        {
            try
            {
                return await db.Clientes.Where(x => x.NumeroDocumento == numeroDocumento
                                                 || x.Nombres == nombre
                                                 && x.Apellidos == apellido
                                                 && x.Estado == 1).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por numero id y documento.
        public async Task<List<Cliente>> ListarClienteUnico(int id, string numeroDocumento)
        {
            try
            {
                return await db.Clientes.Where(x => x.NumeroDocumento == numeroDocumento
                                                 && x.Estado == 1
                                                 && x.IdCliente != id)
                                                     .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //listar por estado
        public async Task<List<Cliente>> ListarClientePorEstado(int estado)
        {
            try
            {
                return await db.Clientes.Where(x => x.Estado == estado).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //buscar por id
        public async Task<Cliente>BuscarClientePorId(int id)
        {
            try
            {
                return await db.Clientes.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //buscar por numero de documento
        public async Task<Cliente> BuscarClientePorNumeroDeIdentidad(string numeroDocumento)
        {
            try
            {
                return await db.Clientes.FindAsync(numeroDocumento);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
