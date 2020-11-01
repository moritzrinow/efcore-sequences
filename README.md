# EntityFrameworkCore.Extensions.Sequences
EntityFrameworkCore extensions for operations on sequences.

**This project is still in the making and highly experimental. Feel free to contribute via PRs.**

# Table of contents
- [Targets](#targets)
- [Installation](#installation)
- [Supported providers](#supported-providers)
- [Usage](#usage)
  - [Configure DbContext](#configure-dbcontext)
  - [Query sequences](#query-sequences)
  - [Manage sequences](#manage-sequences)
    - [Create sequence](#create-sequence)
    - [Alter/Update sequence](#alter/update-sequence)
    - [Drop sequence](#drop-sequence)
- [Limitations](#limitations)

# Targets
- EF Core 3.1

# Installation
## Nuget packages
[EntityFrameworkCore.Extensions.Sequences](https://www.nuget.org/packages/EntityFrameworkCore.Extensions.Sequences)

[EntityFrameworkCore.Extensions.Sequences.SqlServer](https://www.nuget.org/packages/EntityFrameworkCore.Extensions.Sequences.SqlServer)

[EntityFrameworkCore.Extensions.Sequences.PostgreSQL](https://www.nuget.org/packages/EntityFrameworkCore.Extensions.Sequences.PostgreSQL)

# Supported providers
- SqlServer
- PostgreSQL

# Usage
## Configure DbContext
```csharp
optionsBuilder.UseSqlServer("connection", options => options.UseSequences());
```
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
  modelBuilder.UseSequences(this);
}
```
## Query sequences
### Retrieve functional **IQueryable** with these extensions
```csharp
public static IQueryable<IDbSequence<long>> Sequences(this DbContext context)

public static IQueryable<IDbSequence<T>> Sequences<T>(this DbContext context)
```
### Get all sequences in the database
```csharp
using var context = new SampleDbContext()
{
  var sequences = context.Sequences()
                         .ToList();
}
```
### Select specific data
```csharp
var sequences = context.Sequences()
                       .Select(s => new
                       {
                         s.Name,
                         s.Schema,
                         s.CurrentValue
                       }).ToList();
```
### The **IDbSequence** interface
```csharp
public interface IDbSequence<T>
  where T : struct
{
  string Name { get; }

  string Type { get; }

  string Schema { get; }

  string Owner { get; }

  T CurrentValue { get; }

  T StartValue { get; }

  T MinValue { get; }

  T MaxValue { get; }

  T IncrementBy { get; }

  bool Cycle { get; }

  long? CacheSize { get; }
}
```
## Manage sequences
**All functionality can also be used asynchronously**
### Get next sequence value
```csharp
var next = context.NextSequenceValue<long>("mysequence");
```
Within queries (current only for PostgreSQL)
```csharp
var values = this.context.Sequences()
                         .Select(s => new
                         {
                           Name = s.Name,
                           NextValue = EF.Functions.NextValLong(s.Schema + "." + s.Name)
                         }).ToList();
```
### Create sequence
```csharp
bool success = context.CreateSequence(new DbSequence<long>
{
  Name = "mysequence"
});
```
### Alter/Update sequence
```csharp
bool success = context.UpdateSequence("mysequence", new DbSequenceUpdate<long>
{
  RestartWith = 1L
});
```
### Drop sequence
```csharp
bool success = context.DropSequence("mysequence");
```
# Limitations
- IQueryables of type **IDbSequence{T}** can only be used for reading operations and should not be handed to things like bulk-extensions for CRUD.
- The **DbFunctions** extension **EF.Functions.NextVal{Type}(sequence)** cannot translate constant expressions for sequence names and currently only works for PostgreSQL
- **DbContext.NextSequenceValue{T}(sequence)** with PostgreSQL only works for types: **long, decimal**
- Sequences of type **decimal** can only work with whole numbers (1, not 1.5)