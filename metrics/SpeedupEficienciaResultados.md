# Métricas: 

## Evaluación de Speedup y Eficiencia


La siguiente tabla presenta el Speedup y la Eficiencia obtenidos por las tres estrategias paralelas en cada ejecución:

<table>
  <thead>
    <tr>
      <th rowspan="2">Imágenes</th>
      <th colspan="2">Paralelo por filas</th>
      <th colspan="2">División recursiva</th>
      <th colspan="2">Lote paralelo</th>
    </tr>
    <tr>
      <th>Speedup</th>
      <th>Eficiencia</th>
      <th>Speedup</th>
      <th>Eficiencia</th>
      <th>Speedup</th>
      <th>Eficiencia</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><strong>150</strong></td>
      <td>1.81x</td>
      <td>22.67%</td>
      <td>1.59x</td>
      <td>19.84%</td>
      <td>4.37x</td>
      <td>54.63%</td>
    </tr>
    <tr>
      <td><strong>250</strong></td>
      <td>1.72x</td>
      <td>21.50%</td>
      <td>1.53x</td>
      <td>19.13%</td>
      <td>4.05x</td>
      <td>50.59%</td>
    </tr>
    <tr>
      <td><strong>500</strong></td>
      <td>1.61x</td>
      <td>20.09%</td>
      <td>1.34x</td>
      <td>16.81%</td>
      <td>3.48x</td>
      <td>43.50%</td>
    </tr>
    <tr>
      <td><strong>1350</strong></td>
      <td>1.63x</td>
      <td>20.33%</td>
      <td>1.27x</td>
      <td>15.90%</td>
      <td>4.53x</td>
      <td>56.57%</td>
    </tr>
    <tr>
      <td><strong>1950</strong></td>
      <td>1.24x</td>
      <td>15.49%</td>
      <td>1.17x</td>
      <td>14.58%</td>
      <td>2.93x</td>
      <td>36.68%</td>
    </tr>
  </tbody>
</table>


En las primeras tres ejecuciones (*150 a 500 imágenes*) se observa una tendencia decreciente clara en el Speedup de las tres estrategias paralelas conforme crece el volumen: el *"Paralelo por filas"* pasa de *1.81x a 1.61x*, la *"División recursiva"* de *1.59x a 1.34x*, y el *"Lote paralelo"* de *4.37x a 3.48x*. Esto tiene sentido según la Ley de Amdahl: mientras más imágenes hay que procesar, la parte que no se puede paralelizar (leer y guardar cada imagen en disco) ocupa más tiempo del total, y eso deja menos espacio para que el paralelismo siga mejorando el rendimiento.

En la ejecución de *1350 imágenes* se observa una recuperación puntual en el *Lote paralelo*, que alcanza su mejor resultado de toda la evaluación (*4.53x de Speedup, 56.57% de eficiencia*), mientras que el *Paralelo por filas* y la *División recursiva* se mantienen en niveles similares a la ejecución anterior. Sin embargo, en la ejecución de mayor volumen (*1950 imágenes*) las tres estrategias registran sus valores más bajos de Speedup y eficiencia, con el *Lote paralelo* cayendo a *2.93x (36.68%)*, el *Paralelo por filas* a *1.24x (15.49%)* y la *División recursiva* a *1.17x (14.58%)*.

Es importante señalar que el aumento en el tiempo total de la ejecución de 1950 imágenes no fue proporcional al de 1350: aunque se procesó un 44% más de imágenes, el tiempo secuencial solo creció un 5.8%. Esto responde a la variabilidad propia de medir rendimiento en un equipo de uso general, sin que esto reste validez a la tendencia general identificada.