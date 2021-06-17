using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysHotel.EL;
using SysHotel.DAL;
using SysHotel.BL.Service;

namespace SysHotel.BL
{
    public class ClienteBL
    {
        //optimizado
        private ClienteDAL clienteDAL = new ClienteDAL();

        /// <summary>
        /// Agrega un cliente único a la base de datos.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: DUI tiene letras, 3: DUI inválido, 4: El DUI no tiene 9 dígitos, 5: Ciente ya existe, 6: El objeto cliente está incompleto </returns>
        public async Task<int>AgregarClienteUnico(Cliente cliente)
        {
            try
            {
                //Verificamos que el argumento esté completo
                if(!string.IsNullOrEmpty(cliente.Nombres) && !string.IsNullOrEmpty(cliente.Apellidos)
                && cliente.FechaNacimiento != null && !string.IsNullOrEmpty(cliente.TipoDocumento)
                && !string.IsNullOrEmpty(cliente.NumeroDocumento) && !string.IsNullOrEmpty(cliente.Telefono)
                && !string.IsNullOrEmpty(cliente.Correo) && !string.IsNullOrEmpty(cliente.Direccion))
                {
                    //Verificamos que el cliente no se encuentre registrado
                    List<Cliente> ListaClientes = await clienteDAL.ListarClienteUnico(cliente.NumeroDocumento, cliente.Nombres, cliente.Apellidos);
                    int coincidencia = ListaClientes.Count();

                    if(coincidencia == 0)
                    {
                        //Verificacion de DUI
                        int x = VerificarDUI.VerificarNumeroDeDUI(cliente.TipoDocumento, cliente.NumeroDocumento);
                        int y = 0;
                        switch (x)
                        {
                            case 1:
                                y = 2;//El No DUI tiene letras
                                break;

                            case 2:
                                cliente.Estado = 1;
                                y = await clienteDAL.AgregarCliente(cliente);//Cuando es DUI
                                break;

                            case 3:
                                y = 3;//El No de DUI es invalido
                                break;

                            case 4:
                                y = 4;//El DUI no tiene 9 digitos
                                break;

                            case 5:
                                cliente.Estado = 1;
                                y = await clienteDAL.AgregarCliente(cliente);//Cuando no es DUI
                                break;
                        }
                        return y;
                    }
                    return 5;//cliente existe
                }
                return 6;//El objeto cliente viene vacío
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deshabilita el cliente al pasar su estado en 0. Este método no elimina el registro en la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>0: no guardó, 1: guardó, 2: el cliente no existe en la base de datos, 3: el id recibido no es válido, 4: error inesperado.</returns>
        public async Task<int> EliminarCliente(int id)
        {
            try
            {
                if (id > 0)
                {
                    Cliente cliente = await clienteDAL.BuscarClientePorId(id);
                    if (cliente != null)
                    {
                        cliente.Estado = 0;
                        return await clienteDAL.EditarCliente(cliente);
                    }
                    return 2;//El cliente no se encontró en la base de datos
                }
                return 3;//El id es inválido
               
            }
            catch (Exception)
            {
                return 4;//Ocurrió un error inesperado.
            }

        }

        /// <summary>
        /// Edita un cliente asegurando que no se repita en la base de datos.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns>Un entero, donde:
        /// 0: no guardó, 1: guardó, 2: el DUI tiene letras, 3: DUI inválido, 4: DUI incorrecto, 5: Cliente ya existe, 6: No se han hecho cambios, 7: Información incompleta.</returns>
        public async Task<int> EditarCliente(Cliente cliente)
        {
            try    
            {
                //Verificamos que el argumento esté completo
                if (!string.IsNullOrEmpty(cliente.Nombres) || !string.IsNullOrEmpty(cliente.Apellidos)
                || cliente.FechaNacimiento != null || !string.IsNullOrEmpty(cliente.TipoDocumento)
                || !string.IsNullOrEmpty(cliente.NumeroDocumento) || !string.IsNullOrEmpty(cliente.Telefono)
                || !string.IsNullOrEmpty(cliente.Correo) || !string.IsNullOrEmpty(cliente.Direccion))
                {
                    //Control de cambio
                    Cliente clienteExistente = await clienteDAL.BuscarClientePorId(cliente.IdCliente);
                    if (cliente.Nombres != clienteExistente.Nombres || cliente.Apellidos != clienteExistente.Apellidos
                    || cliente.FechaNacimiento != clienteExistente.FechaNacimiento || cliente.TipoDocumento != clienteExistente.TipoDocumento
                    || cliente.NumeroDocumento != clienteExistente.NumeroDocumento || cliente.Telefono != clienteExistente.Telefono
                    || cliente.Correo != clienteExistente.Correo || cliente.Direccion != clienteExistente.Direccion)
                    {
                        //Verificamos que el cliente no se encuentre registrado
                        List<Cliente> ListaClientes = await clienteDAL.ListarClienteUnico(cliente.IdCliente, cliente.NumeroDocumento);
                        int coincidencia = ListaClientes.Count();

                        if (coincidencia == 0)
                        {
                            //Verificacion de DUI
                            int x = VerificarDUI.VerificarNumeroDeDUI(cliente.TipoDocumento, cliente.NumeroDocumento);
                            int y = 0;
                            switch (x)
                            {
                                case 1:
                                    y = 2;//El No DUI tiene letras
                                    break;

                                case 2:
                                    cliente.Estado = 1;
                                    y = await clienteDAL.EditarCliente(cliente);//Cuando es DUI
                                    break;

                                case 3:
                                    y = 3;//El No de DUI es invalido
                                    break;

                                case 4:
                                    y = 4;//El DUI no tiene 9 digitos
                                    break;

                                case 5:
                                    cliente.Estado = 1;
                                    y = await clienteDAL.EditarCliente(cliente);//Cuando no es DUI
                                    break;
                            }
                            return y;
                        }
                        return 5;//cliente existe
                    }
                    return 6;//No se han hecho cambios
                }
                return 7;//El objeto cliente viene vacío
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lista los clientes que se encuentra activos, es decir, estado = 1.
        /// </summary>
        /// <returns>Lista de clientes activos.</returns>
        public async Task<List<Cliente>> ListarClientesActivos()
        {
            try
            {
                List<Cliente> listaClientes = await clienteDAL.ListarClientePorEstado(1);
                return listaClientes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Busca un cliente por id.
        /// </summary>
        /// <returns>El cliente encontrado.</returns>
        public async Task<Cliente> BuscarClientePorId(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await clienteDAL.BuscarClientePorId(id);
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
