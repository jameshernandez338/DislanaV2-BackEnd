# Dislana V2 - Clean Architecture / DDD

## Arquitectura de Dependencias

```
Presentation (Api) → Application → Domain
       ↓                              ↑
       └──────→ Infrastructure ────────┘
```

**✅ CONFIRMADO**: Las dependencias están correctamente establecidas.

### Explicación de Dependencias

1. **Controllers (Presentation)** → Solo dependen de **Application Interfaces** (IOrderService, etc.)
2. **Application** → Depende de **Domain** (entidades, interfaces, value objects)
3. **Infrastructure** → Depende de **Domain** (implementa interfaces de repositorios)
4. **Program.cs (Composition Root)** → Conoce **todas las capas** para configurar DI

**Nota Importante**: El proyecto `Dislana.Api.csproj` tiene referencia tanto a `Application` como a `Infrastructure`. Esto es **correcto** porque `Program.cs` actúa como el **Composition Root** donde se configura la inyección de dependencias, necesitando conocer las implementaciones concretas (Infrastructure) para mapearlas a las interfaces (Domain).

Sin embargo, **los Controllers nunca usan directamente Infrastructure** - solo usan interfaces de Application.

---

## Estado de la Reestructuración

### ✅ Completado

#### **Módulo Order** - 100% Reestructurado
- ✅ Value Objects: `Money`, `Quantity`, `ProductCode`
- ✅ Entidades Ricas: `OrderEntity`, `OrderItemEntity`, `FabricFinishEntity`
- ✅ Lógica de negocio movida de Application → Domain
- ✅ Repository actualizado para trabajar con entidades ricas
- ✅ Application Service simplificado (solo orquestación)
- ✅ Compilación exitosa
- ✅ **Documentación completa**: Ver `ORDER_MODULE_REFACTORING.md`

#### **Módulo Auth** - 100% Reestructurado ✨
- ✅ Value Objects: `Email`, `PersonName`, `HashedPassword`
- ✅ Entidades Ricas: `UserEntity`, `RefreshTokenEntity`, `UserCredentialEntity`
- ✅ Factory Methods: `CreateForRegistration()`, `CreateForNewUser()`, `Create()`, `Reconstitute()`
- ✅ Lógica de negocio movida de Application → Domain (ValidateCanLogin, ValidateForUse, Revoke)
- ✅ Repositorios actualizados para trabajar con entidades ricas
- ✅ Application Service simplificado (solo orquestación)
- ✅ Tests actualizados y pasando (5/5) ✅
- ✅ Compilación exitosa
- ✅ **Documentación completa**: Ver `AUTH_MODULE_REFACTORING.md`

### ⏳ Pendiente de Reestructurar

| Módulo | Prioridad | Estado | Documentación de Referencia |
|--------|-----------|--------|---------------------------|
| **Payment** | 🔴 Alta | Pendiente | `DDD_REFACTORING_GUIDE.md` |
| **Product** | 🟡 Media | Pendiente | `DDD_REFACTORING_GUIDE.md` |
| **Quote** | 🟡 Media | Pendiente | `DDD_REFACTORING_GUIDE.md` |
| **Stock** | 🟢 Baja | Pendiente | `DDD_REFACTORING_GUIDE.md` |

---

## Convenciones de Nombres

### Domain Layer
- **Entidades**: `*Entity` (ej: `OrderEntity`, `UserEntity`, `ProductEntity`)
- **Value Objects**: Nombres descriptivos sin sufijo (ej: `Money`, `ProductCode`, `Email`)
- **Interfaces de Repositorios**: `I*Repository` (ej: `IOrderRepository`)
- **Domain Services**: `*DomainService` (ej: `OrderPricingDomainService`)
- **Results**: `*Result` (ej: `OrderSaveResult`)

### Application Layer
- **DTOs**: `*Dto` (ej: `OrderDto`, `CreateOrderDto`)
- **Application Services**: `*Service` (ej: `OrderService`, `AuthService`)
- **Interfaces**: `I*Service` (ej: `IOrderService`)
- **Results**: `*Result` (ej: `LoginResult`)
- **Requests**: `*Request` (ej: `LoginRequest`, `RegisterRequest`)
- **Responses**: `*Response` o `*ResponseDto` (ej: `LoginResponse`, `OrderSaveResponseDto`)

### Infrastructure Layer
- **Repositories**: `*Repository` (ej: `OrderRepository`, `UserRepository`)
- **Configuration**: `*Options`, `*Settings` (ej: `WompiOptions`, `JwtSettings`)

### Presentation Layer (Api)
- **Controllers**: `*Controller` (ej: `OrdersController`, `AuthController`)

---

## Principios DDD Aplicados

### 1. Entidades Ricas (Rich Domain Model)
Las entidades **NO son anémicas**. Contienen:
- Estado privado con setters privados
- Comportamiento y lógica de negocio
- Factory Methods para construcción
- Validaciones de dominio

```csharp
public class OrderEntity
{
    public string Id { get; private set; }
    private readonly List<OrderItemEntity> _items = new();
    
    private OrderEntity() { } // ORM constructor
    
    public static OrderEntity Create(string userName) 
    {
        // Validaciones y lógica de construcción
    }
    
    public void AddItem(OrderItemEntity item) 
    {
        // Validaciones y lógica de negocio
    }
}
```

### 2. Value Objects
Conceptos sin identidad que se comparan por valor:
- `Money` (cantidad + validaciones)
- `Email` (con validación de formato)
- `ProductCode` (con reglas de negocio)
- `Quantity` (con validación de no negativos)

### 3. Aggregate Roots
- Cada agregado tiene una raíz que controla el acceso
- Solo la raíz puede ser obtenida por repositorio
- Las entidades dentro del agregado solo se acceden vía la raíz

### 4. Application Services
**SOLO orquestan**, NO contienen lógica de negocio:
- Obtienen entidades del repositorio
- Llaman a métodos de las entidades (donde está la lógica)
- Coordinan transacciones
- Mapean entre Domain y DTOs

### 5. Domain Services
Lógica de negocio que no pertenece a una entidad específica:
- Operaciones que involucran múltiples entidades
- Cálculos complejos de dominio

---

## Módulos del Sistema

1. **Auth**: Autenticación, autorización, refresh tokens
2. **Order**: Órdenes de compra con items y acabados ✅ **COMPLETADO**
3. **Payment**: Pagos con integración Wompi, webhooks
4. **Product**: Catálogo de productos, filtros, similares
5. **Quote**: Cotizaciones, impuestos, balance de clientes
6. **Stock**: Inventario, compromisos, estados

---

## Flujo de una Request

```
1. Controller (Api)
   ↓ Recibe Request DTO
   ↓ Valida entrada básica
   ↓ Llama Application Service
   
2. Application Service
   ↓ Mapea DTO → Domain Entity
   ↓ Llama métodos de la entidad (lógica de negocio)
   ↓ Persiste vía Repository
   ↓ Mapea Domain → Response DTO
   
3. Domain Entity
   ↓ Contiene toda la lógica de negocio
   ↓ Valida reglas de dominio
   ↓ Emite eventos de dominio (si aplica)
   
4. Infrastructure Repository
   ↓ Mapea Entity → DB (SP/Dapper)
   ↓ Retorna Entity
```

---

## 📚 Documentación Disponible

| Documento | Propósito | Estado |
|-----------|-----------|--------|
| `ARCHITECTURE.md` | Este documento - Visión general | ✅ Actualizado |
| `ORDER_MODULE_REFACTORING.md` | Reestructuración completa del módulo Order | ✅ Completo |
| `AUTH_MODULE_REFACTORING.md` | Reestructuración completa del módulo Auth | ✅ Completo |
| `DDD_REFACTORING_GUIDE.md` | Guía template para reestructurar otros módulos | ✅ Completo |

---

## 🚀 Próximos Pasos Recomendados

### Fase 1: Módulos Críticos (Alta Prioridad)
1. **Auth**: Usuarios, tokens, autenticación
   - Crear `Email` Value Object
   - Enriquecer `UserEntity` con comportamiento
   - Mover validaciones de passwords al Domain
   - Centralizar lógica de tokens

2. **Payment**: Pagos y webhooks de Wompi
   - Crear `PaymentReference` Value Object
   - Crear `PaymentAmount` Value Object
   - Enriquecer `PaymentEntity` con estados y transiciones
   - Mover lógica de firma/validación al Domain

### Fase 2: Módulos de Negocio (Media Prioridad)
3. **Product**: Catálogo de productos
   - Crear Value Objects para filtros
   - Enriquecer entidades de productos

4. **Quote**: Cotizaciones
   - Centralizar cálculos de impuestos
   - Agregar validaciones de negocio

### Fase 3: Módulos de Soporte (Baja Prioridad)
5. **Stock**: Control de inventario
   - Agregar validaciones de stock
   - Centralizar lógica de compromisos

---

## ✅ Criterios de Éxito Global

El proyecto estará completamente reestructurado cuando:

1. ✅ Todos los módulos tengan entidades ricas
2. ✅ Toda la lógica de negocio esté en Domain
3. ✅ Application Services solo orquesten
4. ✅ Value Objects aplicados consistentemente
5. ✅ Compilación sin errores
6. ✅ Tests pasando
7. ✅ Documentación actualizada

---

## 🎓 Lecciones Aprendidas (Módulo Order)

### Lo que funcionó bien ✅
1. Value Objects simplifican validaciones
2. Factory Methods garantizan consistencia
3. Aggregate Root centraliza acceso
4. Application Service quedó muy simple
5. Retrocompatibilidad mantenida (API sin cambios)

### Patrones aplicados ✅
1. **Aggregate Pattern**: OrderEntity como root
2. **Factory Pattern**: Métodos `Create()` estáticos
3. **Value Object Pattern**: Money, Quantity, ProductCode
4. **Repository Pattern**: Abstracción de persistencia
5. **Rich Domain Model**: Entidades con comportamiento

---

## 📊 Métricas del Proyecto

### Módulo Order (Ejemplo Completado)

**Antes de la reestructuración:**
- Líneas de código en `OrderService`: ~60
- Lógica de negocio en Application: 100%
- Entidades con comportamiento: 0%
- Value Objects: 0

**Después de la reestructuración:**
- Líneas de código en `OrderService`: ~65 (similar, pero más clara)
- Lógica de negocio en Domain: 100%
- Lógica de negocio en Application: 0%
- Entidades con comportamiento: 100%
- Value Objects: 3 (Money, Quantity, ProductCode)
- Métodos de comportamiento agregados: 10+
- Validaciones en Domain: 15+

---

## 🔍 Validación de Arquitectura

### ✅ Dependencias Correctas Confirmadas

```
OrdersController (Presentation)
    ↓ usa
IOrderService (Application Interface)
    ↑ implementado por
OrderService (Application)
    ↓ usa
IOrderRepository (Domain Interface)
    ↑ implementado por
OrderRepository (Infrastructure)
    ↓ usa
OrderEntity, OrderItemEntity (Domain)
```

**✅ Regla de oro**: Domain NO depende de nadie  
**✅ Regla de oro**: Infrastructure depende de Domain  
**✅ Regla de oro**: Application depende de Domain  
**✅ Regla de oro**: Presentation depende de Application

---

## 🎯 Filosofía del Proyecto

> **"El Domain es el corazón del proyecto. Toda la lógica de negocio vive ahí. Application, Infrastructure y Presentation son solo capas de soporte que coordinan, persisten y exponen el Domain."**

---

**Última actualización**: Módulo Order completado y documentado  
**Estado del proyecto**: 2/6 módulos reestructurados (33.33%)  
**Compilación**: ✅ Exitosa  
**Tests**: ⏳ Pendiente de verificación
