# Guía de Reestructuración a Clean Architecture/DDD

## 🎯 Template para Reestructurar Módulos

Esta guía muestra cómo transformar cualquier módulo del proyecto de un **modelo anémico** a **DDD con entidades ricas**.

---

## 📋 Checklist de Reestructuración

### Fase 1: Análisis
- [ ] Identificar entidades del módulo
- [ ] Identificar lógica de negocio actual (dónde está)
- [ ] Identificar validaciones
- [ ] Identificar Value Objects candidatos
- [ ] Identificar el Aggregate Root

### Fase 2: Domain Layer
- [ ] Crear Value Objects necesarios
- [ ] Transformar entidades anémicas en ricas
- [ ] Mover validaciones de Application → Domain
- [ ] Mover lógica de negocio de Application → Domain
- [ ] Crear Factory Methods (`Create()`)
- [ ] Actualizar interfaces de repositorios

### Fase 3: Infrastructure Layer
- [ ] Actualizar repositorios para trabajar con entidades ricas
- [ ] Mantener compatibilidad con SPs existentes

### Fase 4: Application Layer
- [ ] Simplificar Services (solo orquestación)
- [ ] Mantener DTOs sin cambios
- [ ] Mapear DTOs ↔ Entities

### Fase 5: Presentation Layer
- [ ] Verificar que Controllers no cambian (retrocompatibilidad)

### Fase 6: Validación
- [ ] Compilar sin errores
- [ ] Ejecutar tests
- [ ] Verificar funcionalidad

---

## 🔨 Paso a Paso

### 1. Identificar Candidatos a Value Objects

#### ¿Cuándo crear un Value Object?

| Concepto | ¿Es Value Object? | Por qué |
|----------|------------------|---------|
| Dinero | ✅ SÍ | Se compara por valor, tiene validaciones (no negativo) |
| Email | ✅ SÍ | Se compara por valor, tiene formato específico |
| Cantidad | ✅ SÍ | Se compara por valor, tiene validaciones (no negativo) |
| Fecha | ❌ NO | Ya existe `DateTime` en .NET |
| ID de Usuario | ❌ NO | Es identidad, no valor |
| Código de Producto | ✅ SÍ | Tiene formato específico, validaciones |

#### Template de Value Object

```csharp
namespace Dislana.Domain.[Módulo].ValueObjects
{
    public sealed record [Nombre]
    {
        public [Tipo] Value { get; }

        private [Nombre]([Tipo] value)
        {
            Value = value;
        }

        public static [Nombre] Create([Tipo] value)
        {
            // Validaciones aquí
            if ([condición inválida])
                throw new DomainException("[mensaje]");
            
            return new [Nombre](value);
        }

        public static implicit operator [Tipo]([Nombre] obj) => obj.Value;
    }
}
```

#### Ejemplo Real

```csharp
public sealed record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El email es requerido");

        if (!value.Contains("@"))
            throw new DomainException("El email no es válido");

        return new Email(value.Trim().ToLowerInvariant());
    }

    public static implicit operator string(Email email) => email.Value;
}
```

---

### 2. Transformar Entidad Anémica en Rica

#### ANTES (Anémica)
```csharp
public class ProductEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

#### DESPUÉS (Rica)
```csharp
public class ProductEntity
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public int Stock { get; private set; }

    // Constructor privado
    private ProductEntity(string id, string name, Money price, int stock)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    // Factory Method con validaciones
    public static ProductEntity Create(string name, decimal price, int initialStock = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre es requerido");

        if (initialStock < 0)
            throw new DomainException("El stock inicial no puede ser negativo");

        return new ProductEntity(
            Guid.NewGuid().ToString(),
            name.Trim(),
            Money.Create(price),
            initialStock
        );
    }

    // Comportamiento: reducir stock
    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser positiva");

        if (Stock < quantity)
            throw new DomainException($"Stock insuficiente. Disponible: {Stock}, Solicitado: {quantity}");

        Stock -= quantity;
    }

    // Comportamiento: aumentar stock
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser positiva");

        Stock += quantity;
    }

    // Comportamiento: actualizar precio
    public void UpdatePrice(decimal newPrice)
    {
        Price = Money.Create(newPrice);
    }

    // Constructor para ORM
    private ProductEntity() 
    {
        Id = string.Empty;
        Name = string.Empty;
        Price = Money.Zero;
    }
}
```

---

### 3. Mover Lógica de Application Service → Domain

#### ANTES (Lógica en Service)
```csharp
public class ProductService
{
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // ❌ Validaciones en Application
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Nombre requerido");

        if (dto.Price < 0)
            throw new ArgumentException("Precio no puede ser negativo");

        // ❌ Lógica en Application
        var product = new ProductEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.InitialStock
        };

        await _repository.SaveAsync(product);
        return MapToDto(product);
    }

    public async Task ReduceStockAsync(string productId, int quantity)
    {
        var product = await _repository.GetByIdAsync(productId);

        // ❌ Validaciones en Application
        if (product.Stock < quantity)
            throw new InvalidOperationException("Stock insuficiente");

        // ❌ Lógica en Application
        product.Stock -= quantity;
        
        await _repository.UpdateAsync(product);
    }
}
```

#### DESPUÉS (Lógica en Domain)
```csharp
public class ProductService
{
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // ✅ Domain valida y crea
        var product = ProductEntity.Create(dto.Name, dto.Price, dto.InitialStock);

        await _repository.SaveAsync(product);
        return MapToDto(product);
    }

    public async Task ReduceStockAsync(string productId, int quantity)
    {
        var product = await _repository.GetByIdAsync(productId);

        // ✅ Domain valida y ejecuta lógica
        product.ReduceStock(quantity);
        
        await _repository.UpdateAsync(product);
    }
}
```

---

### 4. Actualizar Repository Interface (Domain)

#### ANTES
```csharp
public interface IProductRepository
{
    Task SaveAsync(string name, decimal price, int stock, CancellationToken ct);
    Task UpdateStockAsync(string id, int newStock, CancellationToken ct);
}
```

#### DESPUÉS
```csharp
public interface IProductRepository
{
    Task SaveAsync(ProductEntity product, CancellationToken ct);
    Task<ProductEntity?> GetByIdAsync(string id, CancellationToken ct);
    Task UpdateAsync(ProductEntity product, CancellationToken ct);
}
```

---

### 5. Actualizar Repository Implementation (Infrastructure)

#### ANTES
```csharp
public class ProductRepository : IProductRepository
{
    public async Task SaveAsync(string name, decimal price, int stock, ...)
    {
        await _dbExecutor.ExecuteAsync(
            "usp_saveProduct",
            new { name, price, stock },
            commandType: CommandType.StoredProcedure
        );
    }
}
```

#### DESPUÉS
```csharp
public class ProductRepository : IProductRepository
{
    public async Task SaveAsync(ProductEntity product, ...)
    {
        // La entidad sabe cómo extraer sus datos
        await _dbExecutor.ExecuteAsync(
            "usp_saveProduct",
            new 
            { 
                id = product.Id,
                name = product.Name,
                price = product.Price.Amount,  // Value Object
                stock = product.Stock
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<ProductEntity?> GetByIdAsync(string id, ...)
    {
        // Dapper mapea automáticamente si las propiedades coinciden
        return await _dbExecutor.QuerySingleOrDefaultAsync<ProductEntity>(
            "usp_getProductById",
            new { id },
            commandType: CommandType.StoredProcedure
        );
    }
}
```

---

## 🎨 Patrones DDD Comunes

### 1. Factory Methods

```csharp
// ✅ CORRECTO: Factory Method con validaciones
public static OrderEntity Create(string userName, string? observation)
{
    if (string.IsNullOrWhiteSpace(userName))
        throw new DomainException("Usuario requerido");
    
    return new OrderEntity(userName, observation ?? "");
}

// ❌ INCORRECTO: Constructor público sin validaciones
public OrderEntity(string userName, string observation)
{
    UserName = userName;
    Observation = observation;
}
```

### 2. Encapsulación

```csharp
// ✅ CORRECTO: Estado privado, modificación controlada
private readonly List<OrderItemEntity> _items = new();
public IReadOnlyList<OrderItemEntity> Items => _items.AsReadOnly();

public void AddItem(OrderItemEntity item)
{
    // Validaciones
    if (_items.Count >= 100)
        throw new DomainException("Máximo 100 items");
    _items.Add(item);
}

// ❌ INCORRECTO: Lista pública mutable
public List<OrderItemEntity> Items { get; set; } = new();
```

### 3. Aggregate Boundaries

```csharp
// ✅ CORRECTO: Solo el Aggregate Root se obtiene por repositorio
public interface IOrderRepository
{
    Task<OrderEntity?> GetByIdAsync(string orderId, ...);
    // No hay GetOrderItemAsync - los items solo se acceden vía Order
}

// ❌ INCORRECTO: Repositorios para entidades internas
public interface IOrderItemRepository
{
    Task<OrderItemEntity?> GetByIdAsync(string itemId, ...);
}
```

### 4. Domain Events (Opcional, Avanzado)

```csharp
public class OrderEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Solo órdenes pendientes pueden confirmarse");
        
        Status = OrderStatus.Confirmed;
        
        // Emitir evento de dominio
        _domainEvents.Add(new OrderConfirmedDomainEvent(this.Id));
    }
}
```

---

## 🚨 Errores Comunes a Evitar

### 1. Validar en Application en lugar de Domain

```csharp
// ❌ MAL
public class OrderService
{
    public async Task CreateOrderAsync(...)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Usuario requerido");
        // ...
    }
}

// ✅ BIEN
public class OrderEntity
{
    public static OrderEntity Create(string userName, ...)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("Usuario requerido");
        // ...
    }
}
```

### 2. Exponer Setters Públicos

```csharp
// ❌ MAL
public class ProductEntity
{
    public decimal Price { get; set; }
}

// ✅ BIEN
public class ProductEntity
{
    public Money Price { get; private set; }
    
    public void UpdatePrice(decimal newPrice)
    {
        Price = Money.Create(newPrice);
    }
}
```

### 3. Lógica de Negocio Duplicada

```csharp
// ❌ MAL: Validación duplicada en Service y Entity
public class OrderService
{
    public async Task AddItemAsync(...)
    {
        if (order.Items.Count >= 100)  // Duplicado
            throw new InvalidOperationException("...");
        order.AddItem(item);
    }
}

public class OrderEntity
{
    public void AddItem(OrderItemEntity item)
    {
        if (_items.Count >= 100)  // Duplicado
            throw new DomainException("...");
        _items.Add(item);
    }
}

// ✅ BIEN: Validación solo en Domain
public class OrderService
{
    public async Task AddItemAsync(...)
    {
        order.AddItem(item);  // Domain valida
    }
}
```

### 4. Mezclar Concerns

```csharp
// ❌ MAL: Lógica de persistencia en Entity
public class OrderEntity
{
    public async Task SaveToDatabase()
    {
        using var connection = new SqlConnection(...);
        // ❌ Entity no debe saber de BD
    }
}

// ✅ BIEN: Entity solo tiene lógica de negocio
public class OrderEntity
{
    public string ToXml()  // ✅ OK si es formato de negocio
    {
        return new XDocument(...).ToString();
    }
}
```

---

## 📊 Módulos Pendientes de Reestructurar

| Módulo | Prioridad | Complejidad | Esfuerzo Estimado |
|--------|-----------|-------------|-------------------|
| **Auth** | 🔴 Alta | Media | 2-3 horas |
| **Payment** | 🔴 Alta | Alta | 3-4 horas |
| **Product** | 🟡 Media | Baja | 1-2 horas |
| **Quote** | 🟡 Media | Media | 2-3 horas |
| **Stock** | 🟢 Baja | Baja | 1-2 horas |

---

## ✅ Criterios de Éxito

Una reestructuración exitosa debe cumplir:

1. ✅ **Compilación sin errores**
2. ✅ **Tests pasando** (si existen)
3. ✅ **API pública sin cambios** (retrocompatibilidad)
4. ✅ **Lógica de negocio en Domain**
5. ✅ **Application Service simple** (< 50 líneas por método)
6. ✅ **Entidades con comportamiento**
7. ✅ **Value Objects donde aplique**
8. ✅ **Factory Methods en lugar de constructores públicos**

---

## 📚 Recursos Adicionales

- **Libro**: Domain-Driven Design - Eric Evans
- **Libro**: Implementing Domain-Driven Design - Vaughn Vernon
- **Artículo**: [Martin Fowler - Anemic Domain Model](https://martinfowler.com/bliki/AnemicDomainModel.html)
- **Video**: [Clean Architecture - Uncle Bob](https://www.youtube.com/watch?v=o_TH-Y78tt4)

---

## 🎓 Resumen

### Lo más importante a recordar:

1. **Lógica de negocio va en Domain**, no en Application
2. **Entidades ricas con comportamiento**, no solo propiedades
3. **Value Objects para conceptos sin identidad**
4. **Factory Methods para construcción válida**
5. **Application solo orquesta**, no tiene lógica
6. **Encapsulación**: estado privado, modificación controlada

### Flujo de transformación:

```
1. Identificar lógica de negocio actual
2. Crear Value Objects necesarios
3. Transformar entidades anémicas → ricas
4. Mover validaciones → Domain
5. Mover lógica → Domain
6. Simplificar Application Service
7. Actualizar Repository
8. Compilar y testear
```

---

**¡Buena suerte con la reestructuración!** 🚀

Si tienes dudas, consulta el módulo **Order** como referencia completa.
