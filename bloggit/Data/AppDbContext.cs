using bloggit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bloggit.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    //public DbSet<Product> Products { get; set; }
    public DbSet<Blogs> Blogs { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Reactions> Reactions { get; set; }
    public DbSet<Logs> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Blogs>()
            .HasOne(b => b.User)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.Author)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comments>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Blogs>()
            .HasMany(b => b.Comments)
            .WithOne(c => c.Blog)
            .HasForeignKey(c => c.BlogId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Comments>()
            .HasOne(c => c.ReplyToComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ReplyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Blogs>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Blogs)
            .UsingEntity(j => j.ToTable("BlogTags"));
        
        modelBuilder.Entity<Reactions>()
            .HasOne(r => r.Blog)
            .WithMany(b => b.Reaction)
            .HasForeignKey(r => r.BlogId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reactions>()
            .HasOne(r => r.Comment)
            .WithMany(c => c.Reaction)
            .HasForeignKey(r => r.CommentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Blogs>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Blogs)
            .UsingEntity(j => j.ToTable("BlogTags"));
        
        modelBuilder.Entity<Logs>()
            .HasOne(log => log.Blog)
            .WithMany(blog => blog.Logs)
            .HasForeignKey(log => log.BlogId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Logs>()
            .HasOne(log => log.Comment)
            .WithMany(comment => comment.Logs)
            .HasForeignKey(log => log.CommentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Logs>()
            .HasOne(l => l.User)
            .WithMany(u => u.Logs)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        

            
    }
}