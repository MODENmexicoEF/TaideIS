interface WeatherForecast {
    date: string;
    temperatureC: number;
    summary: string;
}

async function getForecast(): Promise<void> {
    try {
        const response = await fetch('https://localhost:7274/WeatherForecast');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data: WeatherForecast[] = await response.json();
        displayForecast(data);
    } catch (error) {
        console.error('Error fetching forecast:', error);
        const container = document.getElementById('forecast-container');
        if (container) {
            container.textContent = 'Error al obtener el pronóstico.';
        }
    }
}

function displayForecast(forecasts: WeatherForecast[]): void {
    const container = document.getElementById('forecast-container');
    if (container) {
        container.innerHTML = ''; // Limpiar el contenedor anterior
        const ul = document.createElement('ul');
        forecasts.forEach(forecast => {
            const li = document.createElement('li');
            li.textContent = `Fecha: ${forecast.date}, Temperatura (C): ${forecast.temperatureC}, Resumen: ${forecast.summary}`;
            ul.appendChild(li);
        });
        container.appendChild(ul);
    }
}

// Llama a la función para obtener el pronóstico cuando la página se carga
window.onload = getForecast;