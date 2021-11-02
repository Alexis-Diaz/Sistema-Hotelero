using System;
using System.Data.Entity;
using System.Linq;

namespace SysHotel.EL
{
    public class BDComun : DbContext
    {
        //private static string conn = @"workstation id=HotelMyreDB.mssql.somee.com;packet size=4096;user id=Alex45_SQLLogin_1;pwd=q2txuksdt5;data source=HotelMyreDB.mssql.somee.com;persist security info=False;initial catalog=HotelMyreDB";
        private static string conn = @"Data source=DELL-PC\SQLEXPRESS; Initial catalog=HotelBD; Integrated Security=True; App=EntityFramework";
        public BDComun(): base(conn)
        {

        }

        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<TipoHabitacion> TipoHabitacions { get; set; }
        public virtual DbSet<RolUsuario> RolUsuarios { get; set; }
        public virtual DbSet<Permisos> Permisos { get; set; }
        //public virtual DbSet<PermisosDenegadosPorRol> PermisosDenegadoPorRols { get; set; }
        public virtual DbSet<Proveedor> Proveedors { get; set; }
        public virtual DbSet<CategoriaAlimento> CategoriaAlimentos { get; set; }
        public virtual DbSet<Habitacion> Habitacions { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Comentario> Comentarios { get; set; }
        public virtual DbSet<Alimento> Alimentos { get; set; }
        public virtual DbSet<Detalle> Detalles { get; set; }
        public virtual DbSet<Reservacion> Reservacions { get; set; }
        public virtual DbSet<Factura> Facturas { get; set; }


        //Este codigo sirve para personalizar una tabla de union
        //EF crea la tabla de union segun configuracion estadar
        //Aqui solamente personalizamos el nombre
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //se especifican las dos tablas a relacionar
            modelBuilder.Entity<RolUsuario>()
                        .HasMany<Permisos>(p => p.Permisos)
                        .WithMany(r => r.RolUsuario)
                        .Map(rp =>
                        {
                            //aqui se personaliza el nombre de la tabla de union
                            rp.ToTable("PermisosDenegadosPorRol");
                        });
        }
    }
}