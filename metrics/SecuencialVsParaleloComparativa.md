# Métricas

## Comparativa entre ejecución secuencial y paralela

### *Tiempo total por estrategia (ms)*

La siguiente tabla resume el tiempo total que tardó en ejecutarse cada estrategia (o modo) con cinco diferentes volúmenes de imágenes:

| Imágenes | Secuencial (ms) | Paralelo por filas (ms) | División recursiva (ms) | Lote paralelo (ms) |
|---|---|---|---|---|
| **150**  | 5,311.22  | 2,928.79  | 3,347.01  | 1,215.32  | 
| **250**  | 8,764.48  | 5,096.01  | 5,727.18  | 2,165.40  |
| **500**  | 17,325.92 | 10,777.91 | 12,887.04 | 4,978.61  |
| **1350** | 66,573.87 | 40,931.68 | 52,345.24 | 14,709.52 |
| **1950** | 70,459.10 | 56,842.18 | 60,386.87 | 24,012.02 |


En cada una de estas ejecuciones, podemos observar cómo el *"Lote paralelo"* se mantuvo como la estrategia más rápida, seguida por el *"Paralelo por filas"*, mientras que la *"División recursiva"* ocupó de manera 
constante el último lugar entre las estrategias paralelas. La ***diferencia entre el modo Secuencial y el Lote paralelo alcanzó su punto más alto en la ejecución de 1350 imágenes***, donde el tiempo total se redujo en un 77.9% (*de 66,573.87 ms a 14,709.52 ms*).