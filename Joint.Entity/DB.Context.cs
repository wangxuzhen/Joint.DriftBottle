﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Joint.Entity
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class JWDB_JCEntities : DbContext
    {
        public JWDB_JCEntities()
            : base("name=JWDB_JCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bottle> Bottle { get; set; }
        public virtual DbSet<Conversation> Conversation { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<Privileges> Privileges { get; set; }
        public virtual DbSet<RelationPrivilegesRole> RelationPrivilegesRole { get; set; }
        public virtual DbSet<RelationPrivilegesShops> RelationPrivilegesShops { get; set; }
        public virtual DbSet<RelationPrivilegesStores> RelationPrivilegesStores { get; set; }
        public virtual DbSet<RelationPrivilegesSystem> RelationPrivilegesSystem { get; set; }
        public virtual DbSet<RelationPrivilegesUsers> RelationPrivilegesUsers { get; set; }
        public virtual DbSet<RelationRoleModule> RelationRoleModule { get; set; }
        public virtual DbSet<RelationShopsModule> RelationShopsModule { get; set; }
        public virtual DbSet<RelationShopVersionModule> RelationShopVersionModule { get; set; }
        public virtual DbSet<RelationStoresModule> RelationStoresModule { get; set; }
        public virtual DbSet<RelationUserRole> RelationUserRole { get; set; }
        public virtual DbSet<RelationUsersModule> RelationUsersModule { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Shops> Shops { get; set; }
        public virtual DbSet<ShopVersion> ShopVersion { get; set; }
        public virtual DbSet<Stores> Stores { get; set; }
        public virtual DbSet<UserOperationLog> UserOperationLog { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
