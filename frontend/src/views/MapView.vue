<template>
  <div class="map-container">
    <h1>Bike Rental Map</h1>
    <div id="map" class="map-view"></div>

    <div class="controls">
      <button @click="addBikeMarker">Add Bike Marker</button>
      <button @click="addZone">Add Custom Zone</button>
      <button @click="clearOverlays">Clear Overlays</button>
    </div>
    <div class="auth-status">
      <span v-if="isLoggedIn">Logged in as: {{ userEmail }}</span>
      <span v-else>Not logged in. Please <router-link to="/login">login</router-link> to access all
        features.</span>
    </div>
  </div>
</template>

<script>
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { api } from '@/services/api-service'
import { authService } from '@/services/auth-service'

export default {
  name: 'MapView',
  data() {
    return {
      map: null,
      mapCenter: [54.6775530439872, 25.27774382871562], // Vilnius
      overlays: [],
      bikeIcon: null,
      zoneStyle: {
        color: '#3388ff',
        weight: 2,
        opacity: 0.7,
        fillOpacity: 0.2,
        fillColor: '#3388ff',
      },
      zones: [],
      isLoading: false,
      error: null,
      isLoggedIn: false,
      userEmail: null,
    }
  },
  mounted() {
    this.initMap()
    this.createBikeIcon()
    this.checkAuthStatus()

    if (this.isLoggedIn) {
      this.fetchAndDisplayZones()
    }
  },
  methods: {
    checkAuthStatus() {
      this.isLoggedIn = authService.isTokenValid()
      this.userEmail = authService.getUserEmail()
    },

    initMap() {
      this.map = L.map('map').setView(this.mapCenter, 13)

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution:
          '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19,
      }).addTo(this.map)

      this.map.on('click', (event) => {
        console.log('Map clicked at:', event.latlng)
      })
    },

    createBikeIcon() {
      this.bikeIcon = L.divIcon({
        html: `<div class="bike-icon">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" width="24" height="24">
                  <path d="M18.18 10l-1.7-4.68A2.008 2.008 0 0 0 14.6 4H12v2h2.6l1.46 4h-4.81l-.36-1H12V7H7v2h1.75l1.82 5H9.9c-.44-2.23-2.31-3.88-4.65-3.99C2.45 9.87 0 12.2 0 15c0 2.8 2.2 5 5 5 2.46 0 4.45-1.69 4.9-4h4.2c.44 2.23 2.31 3.88 4.65 3.99 2.8.13 5.25-2.19 5.25-5 0-2.8-2.2-5-5-5h-.82zM7.82 16c-.4 1.17-1.49 2-2.82 2-1.68 0-3-1.32-3-3s1.32-3 3-3c1.33 0 2.42.83 2.82 2H5v2h2.82zm6.28-2h-1.4l-.73-2H15c-.44.58-.76 1.25-.9 2zm4.9 4c-1.68 0-3-1.32-3-3 0-.93.41-1.73 1.05-2.28l.96 2.64 1.88-.68-.97-2.67c.03 0 .06-.01.08-.01 1.68 0 3 1.32 3 3s-1.32 3-3 3z"/>
                </svg>
              </div>`,
        className: 'bike-marker-icon',
        iconSize: [30, 30],
        iconAnchor: [15, 15],
        popupAnchor: [0, -15],
      })
    },

    addBikeMarker() {
      const lat = this.mapCenter[0] + (Math.random() - 0.5) * 0.05
      const lng = this.mapCenter[1] + (Math.random() - 0.5) * 0.05

      const marker = L.marker([lat, lng], { icon: this.bikeIcon }).addTo(this.map)

      const popupContent = L.DomUtil.create('div', 'custom-popup')
      popupContent.innerHTML = `
        <h3>Bike #${Math.floor(Math.random() * 1000)}</h3>
        <p>Status: Available</p>
        <p>Battery: 85%</p>
        <button class="popup-button rent-button">Rent Bike</button>
        <button class="popup-button info-button">More Info</button>
      `

      const popup = L.popup().setContent(popupContent)
      marker.bindPopup(popup)

      marker.on('popupopen', () => {
        const rentButton = document.querySelector('.rent-button')
        const infoButton = document.querySelector('.info-button')

        if (rentButton) {
          rentButton.addEventListener('click', () => {
            alert('Bike rental initiated!')
            popup.close()
          })
        }

        if (infoButton) {
          infoButton.addEventListener('click', () => {
            alert('More information about this bike...')
          })
        }
      })

      marker.openPopup()
      this.overlays.push(marker)
    },

    addZone() {
      alert('Click on the map to start drawing a zone. Double-click to finish.')

      const points = []
      let polygon = null
      let tempLine = null

      const handleMapClick = (e) => {
        points.push([e.latlng.lat, e.latlng.lng])

        if (points.length === 1) {
          tempLine = L.polyline(points, this.zoneStyle).addTo(this.map)
        } else {
          tempLine.setLatLngs(points)

          if (e.originalEvent.detail === 2 && points.length >= 3) {
            finishZoneDrawing()
          }
        }
      }

      const finishZoneDrawing = () => {
        this.map.off('click', handleMapClick)

        if (tempLine) {
          this.map.removeLayer(tempLine)
        }

        if (points.length >= 3) {
          polygon = L.polygon(points, this.zoneStyle).addTo(this.map)

          const center = polygon.getBounds().getCenter()
          const zonePopup = L.popup()
            .setLatLng(center)
            .setContent(
              `
              <div class="zone-popup">
                <h3>Custom Zone</h3>
                <div class="color-selection">
                  <label>Zone color:</label>
                  <div class="color-option" style="background-color: #3388ff;" data-color="#3388ff"></div>
                  <div class="color-option" style="background-color: #ff3333;" data-color="#ff3333"></div>
                  <div class="color-option" style="background-color: #33cc33;" data-color="#33cc33"></div>
                </div>
                <label>Zone name:</label>
                <input type="text" class="zone-name" value="Restriction Zone">
                <button class="save-zone-button">Save Zone</button>
              </div>
            `,
            )
            .openOn(this.map)

          polygon.on('popupopen', () => {
            document.querySelectorAll('.color-option').forEach((option) => {
              option.addEventListener('click', (e) => {
                const color = e.target.getAttribute('data-color')
                polygon.setStyle({
                  color: color,
                  fillColor: color,
                })
              })
            })

            const saveButton = document.querySelector('.save-zone-button')
            if (saveButton) {
              saveButton.addEventListener('click', () => {
                const name = document.querySelector('.zone-name').value
                polygon.bindTooltip(name, {
                  permanent: true,
                  direction: 'center',
                  className: 'zone-label',
                })
                zonePopup.close()
              })
            }
          })

          this.overlays.push(polygon)
        }
      }

      this.map.on('click', handleMapClick)
    },

    clearOverlays() {
      this.overlays.forEach((overlay) => {
        this.map.removeLayer(overlay)
      })
      this.overlays = []
    },

    async fetchAndDisplayZones() {
      try {
        this.isLoading = true
        this.error = null
        const response = await api.getZones()
        this.zones = response.data
        this.displayZones()
      } catch (error) {
        console.error('Error fetching zones:', error)
        this.error = 'Failed to fetch zones. Please try again.'
      } finally {
        this.isLoading = false
      }
    },

    displayZones() {
      if (!this.zones || !this.zones.length) return

      this.zones.forEach((zone) => {
        const bounds = [
          [zone.latitude1, zone.longitude1],
          [zone.latitude2, zone.longitude2],
        ]

        const rectangle = L.rectangle(bounds, {
          color: '#3388ff',
          weight: 2,
          opacity: 0.7,
          fillOpacity: 0.2,
          fillColor: '#3388ff',
        }).addTo(this.map)

        const availableBikes = zone.bikes ? zone.bikes.filter(bike => bike.status === 'Available') : []

        rectangle.bindTooltip(`${zone.name}<br>Available bikes: ${availableBikes.length}`, {
          permanent: true,
          direction: 'center',
          className: 'zone-label',
        })

        rectangle.on('click', () => {
          this.showZoneBikes(zone, availableBikes)
        })

        this.overlays.push(rectangle)
      })

      if (this.zones.length > 0) {
        const allBounds = []
        this.zones.forEach((zone) => {
          allBounds.push([zone.latitude1, zone.longitude1])
          allBounds.push([zone.latitude2, zone.longitude2])
        })

        if (allBounds.length > 0) {
          this.map.fitBounds(allBounds)
        }
      }
    },

    rentBike(bikeId) {
      alert(`Starting rental process for bike: ${bikeId}`)
    },

    showBikeInfo(bike) {
      alert(`Bike Details:
Model: ${bike.model}
Status: ${bike.status}
Lock Status: ${bike.lockStatus}
ID: ${bike.id}`)
    },

    showZoneBikes(zone, availableBikes) {
      const center = L.latLngBounds([
        [zone.latitude1, zone.longitude1],
        [zone.latitude2, zone.longitude2]
      ]).getCenter()

      let bikeListHtml = ''
      if (availableBikes.length === 0) {
        bikeListHtml = '<p class="no-bikes">No bikes available in this zone</p>'
      } else {
        bikeListHtml = availableBikes.map(bike => `
          <div class="bike-item" data-bike-id="${bike.id}">
            <div class="bike-info">
              <strong>${bike.model}</strong>
              <span class="bike-price">${bike.rentPrice}€/min</span>
            </div>
            <button class="select-bike-btn" data-bike-id="${bike.id}">Select</button>
          </div>
        `).join('')
      }

      const popupContent = `
        <div class="zone-bikes-popup">
          <h3>${zone.name}</h3>
          <p class="zone-address">${zone.address}</p>
          <p class="bikes-count">Available bikes: ${availableBikes.length}</p>
          <div class="bikes-list">
            ${bikeListHtml}
          </div>
        </div>
      `

      const popup = L.popup({
        maxWidth: 300,
        className: 'zone-bikes-popup-container'
      })
        .setLatLng(center)
        .setContent(popupContent)
        .openOn(this.map)

      setTimeout(() => {
        document.querySelectorAll('.select-bike-btn').forEach(button => {
          button.addEventListener('click', (e) => {
            const bikeId = e.target.getAttribute('data-bike-id')
            const selectedBike = availableBikes.find(bike => bike.id === bikeId)
            this.selectBike(selectedBike, zone)
            popup.close()
          })
        })
      }, 100)
    },

    selectBike(bike, zone) {
      // Reservation popup place
    },
  },
}
</script>

<style scoped>
.map-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.map-view {
  width: 100%;
  height: 500px;
  margin-bottom: 20px;
}

.controls {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.auth-status {
  margin: 10px 0;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 4px;
  color: #000;
}

button {
  padding: 8px 12px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}

/* Custom bike marker styling */
:deep(.bike-marker-icon) {
  background: none;
  border: none;
}

:deep(.bike-icon) {
  color: #ff5722;
  background-color: white;
  border-radius: 50%;
  padding: 3px;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.4);
}

/* Custom popup styling */
:deep(.custom-popup) {
  min-width: 200px;
}

:deep(.popup-button) {
  margin-top: 5px;
  margin-right: 5px;
  padding: 4px 8px;
  border-radius: 4px;
  border: none;
  cursor: pointer;
}

:deep(.rent-button) {
  background-color: #4caf50;
  color: white;
}

:deep(.info-button) {
  background-color: #2196f3;
  color: white;
}

/* Zone styling */
:deep(.zone-popup) {
  min-width: 200px;
}

:deep(.color-selection) {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 10px 0;
}

:deep(.color-option) {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  cursor: pointer;
  border: 1px solid #ccc;
}

:deep(.zone-name) {
  width: 100%;
  padding: 5px;
  margin: 5px 0 10px 0;
}

:deep(.save-zone-button) {
  background-color: #2196f3;
  color: white;
  padding: 5px 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

:deep(.zone-label) {
  background: rgba(255, 255, 255, 0.8);
  border: none;
  border-radius: 4px;
  padding: 5px;
  font-weight: bold;
  text-align: center;
}

/* Zone bikes popup styling */
:deep(.zone-bikes-popup-container .leaflet-popup-content-wrapper) {
  border-radius: 8px;
}

:deep(.zone-bikes-popup) {
  min-width: 250px;
}

:deep(.zone-bikes-popup h3) {
  margin: 0 0 8px 0;
  color: #333;
  font-size: 18px;
}

:deep(.zone-address) {
  margin: 0 0 8px 0;
  color: #666;
  font-size: 14px;
}

:deep(.bikes-count) {
  margin: 0 0 12px 0;
  font-weight: bold;
  color: #2196f3;
}

:deep(.bikes-list) {
  max-height: 200px;
  overflow-y: auto;
}

:deep(.bike-item) {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px;
  margin-bottom: 8px;
  background-color: #f9f9f9;
  border-radius: 4px;
  border: 1px solid #e0e0e0;
}

:deep(.bike-info) {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

:deep(.bike-price) {
  color: #4caf50;
  font-weight: bold;
  font-size: 12px;
}

:deep(.select-bike-btn) {
  background-color: #2196f3;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
}

:deep(.select-bike-btn:hover) {
  background-color: #1976d2;
}

:deep(.no-bikes) {
  color: #666;
  font-style: italic;
  text-align: center;
  padding: 20px;
}
</style>
