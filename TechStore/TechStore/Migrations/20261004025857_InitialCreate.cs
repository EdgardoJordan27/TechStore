using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Descripcion", "Icono", "Nombre" },
                values: new object[,]
                {
                    { 1, "Portátiles gaming, ultrabooks y estaciones de trabajo de alto rendimiento.", "bi-laptop", "Laptops & Computadoras" },
                    { 2, "Teléfonos flagship, gama media y accesorios móviles.", "bi-phone", "Smartphones" },
                    { 3, "Audífonos con cancelación de ruido, parlantes y barras de sonido.", "bi-headphones", "Audio & Sonido" },
                    { 4, "Smartwatches, fitness trackers y dispositivos de salud inteligente.", "bi-smartwatch", "Wearables" },
                    { 5, "Teclados mecánicos, mice, webcams y accesorios RGB.", "bi-keyboard", "Periféricos & Gaming" },
                    { 6, "Monitores ultrawide, 4K y de alta tasa de refresco.", "bi-display", "Monitores & Pantallas" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "Imagen", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, 1, "RTX 4070, 32GB RAM, 1TB SSD, pantalla 165Hz", "Activo", "productos/laptop.jpg", "Laptop HyperX Blaze Pro", 1299.00m, 12 },
                    { 2, 2, "Pantalla AMOLED 120Hz, cámara 108MP, 256GB", "Nuevo", "productos/smartphone.jpg", "Smartphone Nova X1", 899.00m, 25 },
                    { 3, 3, "Cancelación activa de ruido, 40h de batería", "Activo", "productos/audifonos.jpg", "Audífonos SonicPro NC", 249.00m, 8 },
                    { 4, 4, "Monitoreo de salud 24/7, GPS integrado, resistente al agua", "Activo", "productos/smartwatch.jpg", "SmartWatch FitTrack 6", 179.00m, 30 },
                    { 5, 5, "Switches mecánicos, retroiluminación RGB personalizable", "Oferta", "productos/teclado.jpg", "Teclado Mecánico RGB Ducky", 149.00m, 5 },
                    { 6, 6, "Panel IPS, 144Hz, ideal para trabajo y gaming", "Agotado", "productos/monitor.jpg", "Monitor UltraWide 4K 34\"", 599.00m, 0 },
                    { 7, 5, "Grabación 4K a 30fps, enfoque automático, micrófono dual", "Activo", "productos/webcam.jpg", "Webcam 4K StreamPro", 129.00m, 18 },
                    { 8, 5, "63 gramos, sensor óptico 26000 DPI, cable paracord", "Activo", "productos/mouse.jpg", "Mouse Gaming Ultraligero", 79.00m, 42 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
