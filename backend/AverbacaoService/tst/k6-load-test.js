import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '30s', target: 10 },  // Ramp up to 10 virtual users (VUs)
    { duration: '1m', target: 10 },   // Stay at 10 VUs
    { duration: '30s', target: 0 },   // Ramp down to 0
  ],
};

export default function () {
  const url = 'http://averbacao-service:8081/averbacoes/criar';

  const payload = JSON.stringify({
    codigo: Math.floor(Math.random() * 90000) + 10000, // Random number between 10000 and 99999
    convenio: "INSS",
    proponente: {
      cpf: "11111111111",
      nome: "Joana",
      sobrenome: "Silva",
      dataNascimento: "1980-01-01T00:00:00Z"
    },
    valor: 50000.00,
    prazoEmMeses: 36
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  let res = http.post(url, payload, params);

  check(res, {
    'status is 200': r => r.status === 200,
    'response time < 500ms': r => r.timings.duration < 500,
  });

  sleep(1); // Sleep to simulate real user think time
}
