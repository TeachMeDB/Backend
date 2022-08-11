using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace youAreWhatYouEat.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Asset> Assets { get; set; } = null!;
        public virtual DbSet<Attend> Attends { get; set; } = null!;
        public virtual DbSet<Award> Awards { get; set; } = null!;
        public virtual DbSet<CommentOnDish> CommentOnDishes { get; set; } = null!;
        public virtual DbSet<CommentOnService> CommentOnServices { get; set; } = null!;
        public virtual DbSet<Dinningtable> Dinningtables { get; set; } = null!;
        public virtual DbSet<Dish> Dishes { get; set; } = null!;
        public virtual DbSet<Dishorderlist> Dishorderlists { get; set; } = null!;
        public virtual DbSet<Dishtag> Dishtags { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Hasdish> Hasdishes { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<IngredientRecord> IngredientRecords { get; set; } = null!;
        public virtual DbSet<Manage> Manages { get; set; } = null!;
        public virtual DbSet<OrderNumber> OrderNumbers { get; set; } = null!;
        public virtual DbSet<Orderlist> Orderlists { get; set; } = null!;
        public virtual DbSet<OriginChef> OriginChefs { get; set; } = null!;
        public virtual DbSet<OriginIngrRecord> OriginIngrRecords { get; set; } = null!;
        public virtual DbSet<Payroll> Payrolls { get; set; } = null!;
        public virtual DbSet<Prize> Prizes { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<Provide> Provides { get; set; } = null!;
        public virtual DbSet<Repair> Repairs { get; set; } = null!;
        public virtual DbSet<Salary> Salaries { get; set; } = null!;
        public virtual DbSet<Sensor> Sensors { get; set; } = null!;
        public virtual DbSet<Sensorlog> Sensorlogs { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<Vip> Vips { get; set; } = null!;
        public virtual DbSet<WorkPlan> WorkPlans { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle(System.Configuration.ConfigurationManager.ConnectionStrings["OracleDatabase"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("DBKS01");

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.AssetsId)
                    .HasName("SYS_C0011098");

                entity.ToTable("ASSETS");

                entity.Property(e => e.AssetsId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ASSETS_ID");

                entity.Property(e => e.AssetsStatus)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("ASSETS_STATUS");

                entity.Property(e => e.AssetsType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ASSETS_TYPE");

                entity.Property(e => e.EmployeeId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("EMPLOYEE_ID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Assets)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("SYS_C0011099");
            });

            modelBuilder.Entity<Attend>(entity =>
            {
                entity.HasKey(e => new { e.PlanId, e.EmployeeId })
                    .HasName("SYS_C0011086");

                entity.ToTable("ATTEND");

                entity.Property(e => e.PlanId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("PLAN_ID");

                entity.Property(e => e.EmployeeId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("EMPLOYEE_ID");

                entity.Property(e => e.Attendance)
                    .HasPrecision(1)
                    .HasColumnName("ATTENDANCE");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Attends)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("SYS_C0011087");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Attends)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("SYS_C0011088");
            });

            modelBuilder.Entity<Award>(entity =>
            {
                entity.HasKey(e => e.Lv)
                    .HasName("SYS_C0011078");

                entity.ToTable("AWARD");

                entity.Property(e => e.Lv)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LV");

                entity.Property(e => e.Amount)
                    .HasColumnType("NUMBER(9,2)")
                    .HasColumnName("AMOUNT");
            });

            modelBuilder.Entity<CommentOnDish>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("SYS_C0011198");

                entity.ToTable("COMMENT_ON_DISH");

                entity.Property(e => e.CommentId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("COMMENT_ID");

                entity.Property(e => e.CommentContent)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("COMMENT_CONTENT");

                entity.Property(e => e.CommentTime)
                    .HasPrecision(6)
                    .HasColumnName("COMMENT_TIME");

                entity.Property(e => e.DishId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DISH_ID");

                entity.Property(e => e.Stars)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("STARS");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.CommentOnDishes)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("SYS_C0011200");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.CommentOnDishes)
                    .HasForeignKey(d => d.UserName)
                    .HasConstraintName("SYS_C0011199");
            });

            modelBuilder.Entity<CommentOnService>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("SYS_C0011204");

                entity.ToTable("COMMENT_ON_SERVICE");

                entity.Property(e => e.CommentId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("COMMENT_ID");

                entity.Property(e => e.CommentContent)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("COMMENT_CONTENT");

                entity.Property(e => e.CommentTime)
                    .HasPrecision(6)
                    .HasColumnName("COMMENT_TIME");

                entity.Property(e => e.Stars)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("STARS");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.CommentOnServices)
                    .HasForeignKey(d => d.UserName)
                    .HasConstraintName("SYS_C0011205");
            });

            modelBuilder.Entity<Dinningtable>(entity =>
            {
                entity.HasKey(e => e.TableId)
                    .HasName("SYS_C0011148");

                entity.ToTable("DINNINGTABLE");

                entity.Property(e => e.TableId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("TABLE_ID");

                entity.Property(e => e.CustomerNumber)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("CUSTOMER_NUMBER");

                entity.Property(e => e.Occupied)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OCCUPIED");

                entity.Property(e => e.TableCapacity)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("TABLE_CAPACITY");
            });

            modelBuilder.Entity<Dish>(entity =>
            {
                entity.ToTable("DISHES");

                entity.Property(e => e.DishId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DISH_ID");

                entity.Property(e => e.DishDescription)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("DISH_DESCRIPTION");

                entity.Property(e => e.DishName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISH_NAME");

                entity.Property(e => e.DishPrice)
                    .HasColumnType("NUMBER(9,2)")
                    .HasColumnName("DISH_PRICE");

                entity.HasMany(d => d.Chefs)
                    .WithMany(p => p.Dishes)
                    .UsingEntity<Dictionary<string, object>>(
                        "ChefCancookDish",
                        l => l.HasOne<Employee>().WithMany().HasForeignKey("ChefId").HasConstraintName("SYS_C0011126"),
                        r => r.HasOne<Dish>().WithMany().HasForeignKey("DishId").HasConstraintName("SYS_C0011125"),
                        j =>
                        {
                            j.HasKey("DishId", "ChefId").HasName("SYS_C0011124");

                            j.ToTable("CHEF_CANCOOK_DISH");

                            j.IndexerProperty<decimal>("DishId").HasColumnType("NUMBER(38)").HasColumnName("DISH_ID");

                            j.IndexerProperty<decimal>("ChefId").HasColumnType("NUMBER(38)").HasColumnName("CHEF_ID");
                        });

                entity.HasMany(d => d.Dtags)
                    .WithMany(p => p.Dishes)
                    .UsingEntity<Dictionary<string, object>>(
                        "DishHasTag",
                        l => l.HasOne<Dishtag>().WithMany().HasForeignKey("DtagId").HasConstraintName("SYS_C0011123"),
                        r => r.HasOne<Dish>().WithMany().HasForeignKey("DishId").HasConstraintName("SYS_C0011122"),
                        j =>
                        {
                            j.HasKey("DishId", "DtagId").HasName("SYS_C0011121");

                            j.ToTable("DISH_HAS_TAG");

                            j.IndexerProperty<decimal>("DishId").HasColumnType("NUMBER(38)").HasColumnName("DISH_ID");

                            j.IndexerProperty<decimal>("DtagId").HasColumnType("NUMBER(38)").HasColumnName("DTAG_ID");
                        });

                entity.HasMany(d => d.Ingrs)
                    .WithMany(p => p.Dishes)
                    .UsingEntity<Dictionary<string, object>>(
                        "DisheNeedIngr",
                        l => l.HasOne<Ingredient>().WithMany().HasForeignKey("IngrId").HasConstraintName("SYS_C0011129"),
                        r => r.HasOne<Dish>().WithMany().HasForeignKey("DishId").HasConstraintName("SYS_C0011128"),
                        j =>
                        {
                            j.HasKey("DishId", "IngrId").HasName("SYS_C0011127");

                            j.ToTable("DISHE_NEED_INGR");

                            j.IndexerProperty<decimal>("DishId").HasColumnType("NUMBER(38)").HasColumnName("DISH_ID");

                            j.IndexerProperty<decimal>("IngrId").HasColumnType("NUMBER(38)").HasColumnName("INGR_ID");
                        });
            });

            modelBuilder.Entity<Dishorderlist>(entity =>
            {
                entity.HasKey(e => e.DishOrderId)
                    .HasName("SYS_C0011162");

                entity.ToTable("DISHORDERLIST");

                entity.Property(e => e.DishOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISH_ORDER_ID");

                entity.Property(e => e.DishId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DISH_ID");

                entity.Property(e => e.DishStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DISH_STATUS");

                entity.Property(e => e.FinalPayment)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("FINAL_PAYMENT");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_ID");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.Dishorderlists)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("SYS_C0011164");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Dishorderlists)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("SYS_C0011163");
            });

            modelBuilder.Entity<Dishtag>(entity =>
            {
                entity.HasKey(e => e.DtagId)
                    .HasName("SYS_C0011113");

                entity.ToTable("DISHTAGS");

                entity.Property(e => e.DtagId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DTAG_ID");

                entity.Property(e => e.DtagName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DTAG_NAME");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("EMPLOYEE");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("ID");

                entity.Property(e => e.Birthday)
                    .HasColumnType("DATE")
                    .HasColumnName("BIRTHDAY");

                entity.Property(e => e.Gender)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("GENDER");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Occupation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("OCCUPATION");

                entity.HasOne(d => d.OccupationNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.Occupation)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011083");
            });

            modelBuilder.Entity<Hasdish>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.DishId })
                    .HasName("SYS_C0011184");

                entity.ToTable("HASDISH");

                entity.Property(e => e.PromotionId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("PROMOTION_ID");

                entity.Property(e => e.DishId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DISH_ID");

                entity.Property(e => e.Discount)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("DISCOUNT");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.Hasdishes)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("SYS_C0011186");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.Hasdishes)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("SYS_C0011185");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.IngrId)
                    .HasName("SYS_C0011116");

                entity.ToTable("INGREDIENTS");

                entity.Property(e => e.IngrId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("INGR_ID");

                entity.Property(e => e.IngrDescription)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("INGR_DESCRIPTION");

                entity.Property(e => e.IngrName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("INGR_NAME");

                entity.Property(e => e.IngrType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("INGR_TYPE");
            });

            modelBuilder.Entity<IngredientRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("SYS_C0011135");

                entity.ToTable("INGREDIENT_RECORD");

                entity.Property(e => e.RecordId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("RECORD_ID");

                entity.Property(e => e.DirectorId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DIRECTOR_ID");

                entity.Property(e => e.IngrId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("INGR_ID");

                entity.Property(e => e.MeasureUnit)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MEASURE_UNIT");

                entity.Property(e => e.Price)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.ProducedDate)
                    .HasColumnType("DATE")
                    .HasColumnName("PRODUCED_DATE");

                entity.Property(e => e.Purchases)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("PURCHASES");

                entity.Property(e => e.PurchasingDate)
                    .HasColumnType("DATE")
                    .HasColumnName("PURCHASING_DATE");

                entity.Property(e => e.ShelfLife)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("SHELF_LIFE");

                entity.Property(e => e.Surplus)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("SURPLUS");

                entity.HasOne(d => d.Director)
                    .WithMany(p => p.IngredientRecords)
                    .HasForeignKey(d => d.DirectorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011136");

                entity.HasOne(d => d.Ingr)
                    .WithMany(p => p.IngredientRecords)
                    .HasForeignKey(d => d.IngrId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011137");
            });

            modelBuilder.Entity<Manage>(entity =>
            {
                entity.HasKey(e => new { e.AssetsId, e.ManageType, e.ManageDate, e.ManageReason, e.ManageCost })
                    .HasName("SYS_C0011105");

                entity.ToTable("MANAGE");

                entity.Property(e => e.AssetsId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ASSETS_ID");

                entity.Property(e => e.ManageType)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("MANAGE_TYPE");

                entity.Property(e => e.ManageDate)
                    .HasColumnType("DATE")
                    .HasColumnName("MANAGE_DATE");

                entity.Property(e => e.ManageReason)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MANAGE_REASON");

                entity.Property(e => e.ManageCost)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("MANAGE_COST");

                entity.Property(e => e.EmployeeId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("EMPLOYEE_ID");

                entity.Property(e => e.Repair)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("REPAIR");

                entity.HasOne(d => d.Assets)
                    .WithMany(p => p.Manages)
                    .HasForeignKey(d => d.AssetsId)
                    .HasConstraintName("SYS_C0011107");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Manages)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("SYS_C0011106");

                entity.HasOne(d => d.RepairNavigation)
                    .WithMany(p => p.Manages)
                    .HasForeignKey(d => d.Repair)
                    .HasConstraintName("FK_REPAIR");
            });

            modelBuilder.Entity<OrderNumber>(entity =>
            {
                entity.HasKey(e => new { e.OrderDate, e.OrderNumber1 })
                    .HasName("SYS_C0011176");

                entity.ToTable("ORDER_NUMBER");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("DATE")
                    .HasColumnName("ORDER_DATE");

                entity.Property(e => e.OrderNumber1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_NUMBER");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderNumbers)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011178");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.OrderNumbers)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011177");
            });

            modelBuilder.Entity<Orderlist>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("SYS_C0011154");

                entity.ToTable("ORDERLIST");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.CreationTime)
                    .HasColumnType("DATE")
                    .HasColumnName("CREATION_TIME");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_STATUS");

                entity.Property(e => e.TableId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("TABLE_ID");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.Orderlists)
                    .HasForeignKey(d => d.TableId)
                    .HasConstraintName("SYS_C0011155");
            });

            modelBuilder.Entity<OriginChef>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ORIGIN_CHEF");

                entity.Property(e => e.ChefId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("CHEF_ID");

                entity.Property(e => e.DishOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISH_ORDER_ID");

                entity.HasOne(d => d.Chef)
                    .WithMany()
                    .HasForeignKey(d => d.ChefId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011189");

                entity.HasOne(d => d.DishOrder)
                    .WithMany()
                    .HasForeignKey(d => d.DishOrderId)
                    .HasConstraintName("SYS_C0011188");
            });

            modelBuilder.Entity<OriginIngrRecord>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ORIGIN_INGR_RECORD");

                entity.Property(e => e.DishOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISH_ORDER_ID");

                entity.Property(e => e.RecordId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("RECORD_ID");

                entity.HasOne(d => d.DishOrder)
                    .WithMany()
                    .HasForeignKey(d => d.DishOrderId)
                    .HasConstraintName("SYS_C0011192");

                entity.HasOne(d => d.Record)
                    .WithMany()
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("SYS_C0011193");
            });

            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.HasKey(e => new { e.PayDatetime, e.EmployeeId })
                    .HasName("SYS_C0011089");

                entity.ToTable("PAYROLL");

                entity.Property(e => e.PayDatetime)
                    .HasColumnType("DATE")
                    .HasColumnName("PAY_DATETIME");

                entity.Property(e => e.EmployeeId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("EMPLOYEE_ID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Payrolls)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("SYS_C0011090");
            });

            modelBuilder.Entity<Prize>(entity =>
            {
                entity.HasKey(e => new { e.PrizeDatetime, e.EmployeeId, e.Lv })
                    .HasName("SYS_C0011091");

                entity.ToTable("PRIZE");

                entity.Property(e => e.PrizeDatetime)
                    .HasColumnType("DATE")
                    .HasColumnName("PRIZE_DATETIME");

                entity.Property(e => e.EmployeeId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("EMPLOYEE_ID");

                entity.Property(e => e.Lv)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LV");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Prizes)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("SYS_C0011092");

                entity.HasOne(d => d.LvNavigation)
                    .WithMany(p => p.Prizes)
                    .HasForeignKey(d => d.Lv)
                    .HasConstraintName("SYS_C0011093");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("PROMOTION");

                entity.Property(e => e.PromotionId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("PROMOTION_ID");

                entity.Property(e => e.Description)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.EndTime)
                    .HasColumnType("DATE")
                    .HasColumnName("END_TIME");

                entity.Property(e => e.StartTime)
                    .HasColumnType("DATE")
                    .HasColumnName("START_TIME");
            });

            modelBuilder.Entity<Provide>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("SYS_C0011143");

                entity.ToTable("PROVIDE");

                entity.Property(e => e.RecordId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("RECORD_ID");

                entity.Property(e => e.SId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("S_ID");

                entity.HasOne(d => d.Record)
                    .WithOne(p => p.Provide)
                    .HasForeignKey<Provide>(d => d.RecordId)
                    .HasConstraintName("SYS_C0011144");

                entity.HasOne(d => d.SIdNavigation)
                    .WithMany(p => p.Provides)
                    .HasForeignKey(d => d.SId)
                    .HasConstraintName("SYS_C0011145");
            });

            modelBuilder.Entity<Repair>(entity =>
            {
                entity.ToTable("REPAIR");

                entity.Property(e => e.RepairId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("REPAIR_ID");

                entity.Property(e => e.Latitude)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("LATITUDE");

                entity.Property(e => e.Longitude)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("LONGITUDE");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Phone)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");
            });

            modelBuilder.Entity<Salary>(entity =>
            {
                entity.HasKey(e => e.Occupation)
                    .HasName("SYS_C0011080");

                entity.ToTable("SALARY");

                entity.Property(e => e.Occupation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("OCCUPATION");

                entity.Property(e => e.Amount)
                    .HasColumnType("NUMBER(9,2)")
                    .HasColumnName("AMOUNT");
            });

            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.HasKey(e => e.SensId)
                    .HasName("SYS_C0011120");

                entity.ToTable("SENSORS");

                entity.Property(e => e.SensId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("SENS_ID");

                entity.Property(e => e.SensLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SENS_LOCATION");

                entity.Property(e => e.SensModel)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SENS_MODEL");

                entity.Property(e => e.SensType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SENS_TYPE");
            });

            modelBuilder.Entity<Sensorlog>(entity =>
            {
                entity.HasKey(e => e.SlogId)
                    .HasName("SYS_C0011132");

                entity.ToTable("SENSORLOG");

                entity.Property(e => e.SlogId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("SLOG_ID");

                entity.Property(e => e.SensId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("SENS_ID");

                entity.Property(e => e.SlogTime)
                    .HasPrecision(6)
                    .HasColumnName("SLOG_TIME");

                entity.Property(e => e.SlogValue)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("SLOG_VALUE");

                entity.HasOne(d => d.Sens)
                    .WithMany(p => p.Sensorlogs)
                    .HasForeignKey(d => d.SensId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011133");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("SYS_C0011139");

                entity.ToTable("SUPPLIER");

                entity.Property(e => e.SId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("S_ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.Contact)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("CONTACT");

                entity.Property(e => e.DirectorId)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("DIRECTOR_ID");

                entity.Property(e => e.SName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("S_NAME");

                entity.HasOne(d => d.Director)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.DirectorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011140");
            });

            modelBuilder.Entity<Vip>(entity =>
            {
                entity.HasKey(e => e.UserName)
                    .HasName("SYS_C0011173");

                entity.ToTable("VIP");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");

                entity.Property(e => e.Balance)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("BALANCE")
                    .HasDefaultValueSql("(0.0) ");

                entity.Property(e => e.Birthday)
                    .HasColumnType("DATE")
                    .HasColumnName("BIRTHDAY");

                entity.Property(e => e.Credit)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("CREDIT")
                    .HasDefaultValueSql("(0) ");

                entity.Property(e => e.Gender)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("GENDER");

                entity.Property(e => e.IsDefault)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("IS_DEFAULT")
                    .HasDefaultValueSql("('是') ");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");
            });

            modelBuilder.Entity<WorkPlan>(entity =>
            {
                entity.ToTable("WORK_PLAN");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("ID");

                entity.Property(e => e.No)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("NO");

                entity.Property(e => e.Occupation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("OCCUPATION");

                entity.Property(e => e.Place)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PLACE");

                entity.Property(e => e.TimeEnd)
                    .HasColumnType("DATE")
                    .HasColumnName("TIME_END");

                entity.Property(e => e.TimeStart)
                    .HasColumnType("DATE")
                    .HasColumnName("TIME_START");

                entity.HasOne(d => d.OccupationNavigation)
                    .WithMany(p => p.WorkPlans)
                    .HasForeignKey(d => d.Occupation)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0011085");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
