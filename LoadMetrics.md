# Test de carga del programa
Se realizo un test de carga del utilizando **Grafana K6**  utilizando el script `spike_tests.js` y ejecutando el comando `k6 run --out web-dashboard .\spike_tests.js`

## Metricas del test de carga
```

         /\      Grafana   /‾‾/
    /\  /  \     |\  __   /  /
   /  \/    \    | |/ /  /   ‾‾\
  /          \   |   (  |  (‾)  |
 / __________ \  |_|\_\  \_____/

     execution: local
        script: .\spike_tests.js
 web dashboard: http://127.0.0.1:5665
        output: -

     scenarios: (100.00%) 1 scenario, 100 max VUs, 1m10s max duration (incl. graceful stop):
              * default: Up to 100 looping VUs for 40s over 3 stages (gracefulRampDown: 30s, gracefulStop: 30s)



  █ TOTAL RESULTS

    checks_total.......: 17860   446.344665/s
    checks_succeeded...: 100.00% 17860 out of 17860
    checks_failed......: 0.00%   0 out of 17860

    ✓ response is valid
    ✓ no server errors

    CUSTOM
    status_codes...................: avg=200      min=200     med=200      max=200   p(90)=200      p(95)=200
    successful_requests............: 8930   223.172333/s

    HTTP
    http_req_duration..............: avg=337.28ms min=23.17ms med=334.8ms  max=1.07s p(90)=575.8ms  p(95)=662.05ms
      { expected_response:true }...: avg=337.28ms min=23.17ms med=334.8ms  max=1.07s p(90)=575.8ms  p(95)=662.05ms
    http_req_failed................: 0.00%  0 out of 8930
    http_reqs......................: 8930   223.172333/s

    EXECUTION
    iteration_duration.............: avg=337.98ms min=23.17ms med=335.44ms max=1.07s p(90)=577.02ms p(95)=663ms
    iterations.....................: 8930   223.172333/s
    vus............................: 1      min=1         max=100
    vus_max........................: 100    min=100       max=100

    NETWORK
    data_received..................: 338 MB 8.5 MB/s
    data_sent......................: 1.1 MB 28 kB/s
```
## Analisis de las métricas
El test muestra un comportamiento altamente estable bajo carga:

- 0% errores HTTP
- 100% de respuestas 200
- 8.930 solicitudes en ~40 segundos
- 223 req/s sostenidos
- Procesamiento estable incluso con 100 VUs
- p95 cercano a 660 ms es aceptable, pero si buscas <300 ms hay margen de mejora
- Generar PDFs es intensivo en CPU → más concurrencia podría empezar a saturar el servidor

Para una API que genera PDFs (operación CPU-intensiva), los resultados son sólidos y robustos.


