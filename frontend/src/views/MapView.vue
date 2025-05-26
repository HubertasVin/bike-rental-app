<template>
  <div class="map-container">
    <!-- Floating User Icon -->
    <div v-if="isLoggedIn" class="floating-user-icon" @click="goToUserPage">
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" width="24" height="24">
        <path
          d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
      </svg>
    </div>

    <!-- Toast Notifications -->
    <div v-if="toastMessage" class="toast-notification" :class="toastType">
      <div class="toast-content">
        <span>{{ toastMessage }}</span>
        <button @click="closeToast" class="toast-close">×</button>
      </div>
    </div>

    <!-- Confirmation Modal -->
    <div v-if="showConfirmModal" class="modal-overlay" @click="closeConfirmModal">
      <div class="confirmation-modal" @click.stop>
        <h3>{{ confirmModal.title }}</h3>
        <p>{{ confirmModal.message }}</p>
        <div class="modal-actions">
          <button @click="closeConfirmModal" class="cancel-btn-modal">Cancel</button>
          <button @click="confirmAction" class="confirm-btn-modal">{{ confirmModal.confirmText }}</button>
        </div>
      </div>
    </div>

    <!-- Problem Report Modal -->
    <div v-if="showProblemModal" class="modal-overlay" @click="closeProblemModal">
      <div class="problem-modal" @click.stop>
        <h3>Report a Problem</h3>
        <div class="problem-options">
          <button v-for="(problem, index) in problemTypes" :key="index" @click="selectProblem(problem)"
            class="problem-option">
            {{ problem }}
          </button>
        </div>
        <button @click="closeProblemModal" class="cancel-btn-modal">Cancel</button>
      </div>
    </div>

    <h1 class="desktop-only">Bike Rental Map</h1>
    <div id="map" class="map-view"></div>

    <div class="controls desktop-only">
      <button @click="addBikeMarker">Add Bike Marker</button>
      <button @click="addZone">Add Custom Zone</button>
      <button @click="clearOverlays">Clear Overlays</button>
    </div>
    <div class="auth-status desktop-only">
      <span v-if="isLoggedIn">Logged in as: {{ userEmail }}</span>
      <span v-else>Not logged in. Please <router-link to="/login">login</router-link> to access all
        features.</span>
    </div>

    <!-- Zone Bikes Bottom Popup -->
    <div v-if="showZonePopup" class="bottom-popup zone-popup-overlay" :class="{ 'full-screen': isZonePopupExpanded }"
      @click="closeZonePopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isZonePopupExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="toggleZonePopupExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>{{ selectedZone?.name }}</h3>
          <button class="close-btn" @click="closeZonePopup">×</button>
        </div>
        <p class="zone-subtitle">{{ selectedZone?.address }}</p>
        <p class="bikes-count">Available bikes: {{ availableBikes.length }}</p>

        <div v-if="availableBikes.length === 0" class="no-bikes">
          <div class="no-bikes-content">
            <p>No bikes available in this zone</p>
          </div>
        </div>

        <div v-else class="bikes-container">
          <div v-for="bike in availableBikes" :key="bike.id" class="bike-card" @click="selectBike(bike)">
            <div class="bike-card-content">
              <h4 class="bike-name">{{ bike.model }}</h4>
              <div class="bike-details">
                <div class="bike-detail">
                  <span>Price per minute:</span>
                  <span class="price">{{ bike.pricePerMinute }}€</span>
                </div>
              </div>
            </div>
            <button class="select-bike-btn">
              <span class="icon">▶</span>
              Select bike
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Reservation Bottom Popup -->
    <div v-if="showReservationPopup" class="bottom-popup reservation-popup-overlay"
      :class="{ 'full-screen': isReservationPopupExpanded }" @click="closeReservationPopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isReservationPopupExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="toggleReservationPopupExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>Reserve a bike</h3>
          <button class="close-btn" @click="closeReservationPopup">×</button>
        </div>
        <p class="bike-model">{{ selectedBike?.model }}</p>
        <div class="reservation-details">
          <div class="reservation-detail">
            <span>Price per minute:</span>
            <span>{{ selectedBike?.pricePerMinute }}€</span>
          </div>
          <div class="reservation-detail">
            <span>Reservation time:</span>
            <span>15 min.</span>
          </div>
        </div>
        <button class="begin-reservation-btn" @click="beginBikeReservation">
          <span class="icon">▶</span>
          Begin reservation
        </button>
      </div>
    </div>

    <!-- Active Reservation Status Popup -->
    <div v-if="showActiveReservationPopup" class="bottom-popup reservation-status-overlay"
      :class="{ 'full-screen': isActiveReservationExpanded }" @click="closeActiveReservationPopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isActiveReservationExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="toggleActiveReservationExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>Bike reservation</h3>
          <button class="close-btn" @click="closeActiveReservationPopup">×</button>
        </div>
        <p class="bike-model">{{ activeReservation?.bikeModel }}</p>

        <div class="reservation-status">
          <div class="status-item">
            <span>Current price:</span>
            <span class="cost">{{ currentCost }}€</span>
          </div>
          <div class="status-item">
            <span>{{ isReservationFree ? 'Reservation time left:' : 'Paid reserved time:' }}</span>
            <span class="time-left">{{ timeLeft }}</span>
          </div>
        </div>

        <div class="action-buttons">
          <button class="unlock-bike-btn" @click="unlockBike">
            <span class="unlock-icon">🔓</span>
            Unlock bike
          </button>

          <button v-if="isReservationFree" class="cancel-reservation-btn" @click="cancelReservation">
            <span class="cancel-icon">❌</span>
            Cancel reservation
          </button>

          <div class="report-problem-link" @click="reportProblem">
            Want to report a problem?
          </div>
        </div>

        <div class="reservation-info">
          <p v-if="isReservationFree" class="info-text">
            After reservation time, you will pay per minute.
          </p>
          <p v-else class="info-text">
            You are currently being charged per minute.
          </p>
        </div>
      </div>
    </div>

    <!-- Ongoing Ride Popup -->
    <div v-if="showOngoingRidePopup" class="bottom-popup ride-popup-overlay"
      :class="{ 'full-screen': isOngoingRideExpanded }" @click="closeOngoingRidePopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isOngoingRideExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="toggleOngoingRideExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>Ongoing ride</h3>
          <button class="close-btn" @click="closeOngoingRidePopup">×</button>
        </div>

        <div class="ride-status">
          <div class="status-item">
            <span>Current price:</span>
            <span class="cost">{{ currentRideCost }}€</span>
          </div>
          <div class="status-item">
            <span>Ride time:</span>
            <span class="time-left">{{ rideTime }}</span>
          </div>
          <div class="status-item">
            <span>Bike status:</span>
            <span class="lock-status" :class="{ 'locked': isBikeLocked, 'unlocked': !isBikeLocked }">
              {{ isBikeLocked ? '🔒 Locked' : '🔓 Unlocked' }}
            </span>
          </div>
        </div>

        <div class="action-buttons">
          <button v-if="!isBikeLocked" class="lock-bike-btn" @click="lockBike">
            <span class="lock-icon">🔒</span>
            Lock bike
          </button>

          <button v-if="isBikeLocked" class="unlock-bike-btn" @click="unlockBikeRental">
            <span class="unlock-icon">🔓</span>
            Unlock bike
          </button>

          <button class="finish-ride-btn" @click="finishRide">
            <span class="stop-icon">⏹</span>
            Finish ride
          </button>

          <div class="report-problem-link" @click="reportProblem">
            Want to report a problem?
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Popup -->
    <div v-if="showPaymentPopup" class="bottom-popup payment-popup-overlay"
      :class="{ 'full-screen': isPaymentExpanded }" @click="closePaymentPopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isPaymentExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="togglePaymentExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>Ride completed</h3>
          <button class="close-btn" @click="closePaymentPopup">×</button>
        </div>

        <div class="payment-status">
          <div class="status-item">
            <span>Final price:</span>
            <span class="cost">{{ finalRideCost }}€</span>
          </div>
          <div class="status-item">
            <span>Total ride time:</span>
            <span class="time-left">{{ finalRideTime }}</span>
          </div>

          <!-- Cost Breakdown -->
          <div v-if="costBreakdown.showBreakdown" class="cost-breakdown">
            <div class="breakdown-header">Cost Breakdown:</div>
            <div class="breakdown-item" v-if="costBreakdown.reservationCost > 0">
              <span>Reservation cost:</span>
              <span class="breakdown-cost">{{ costBreakdown.reservationCost.toFixed(2) }}€</span>
            </div>
            <div class="breakdown-item">
              <span>Ride cost:</span>
              <span class="breakdown-cost">{{ costBreakdown.rideCost.toFixed(2) }}€</span>
            </div>
            <div class="breakdown-item total-line">
              <span>Total:</span>
              <span class="breakdown-total">{{ (costBreakdown.reservationCost + costBreakdown.rideCost).toFixed(2)
              }}€</span>
            </div>
          </div>
        </div>
      </div>
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
      mapCenter: [54.6775530439872, 25.27774382871562],
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
      showZonePopup: false,
      showReservationPopup: false,
      selectedZone: null,
      selectedBike: null,
      availableBikes: [],
      isZonePopupExpanded: false,
      isReservationPopupExpanded: false,
      isDragging: false,
      dragStartY: 0,
      dragCurrentY: 0,
      dragThreshold: 100,
      isMouseDragging: false,
      activeReservation: null,
      showActiveReservationPopup: false,
      isActiveReservationExpanded: false,
      currentCost: '0.00',
      isReservationFree: true,
      timeLeft: '00:00',
      reservationTimer: null,
      costUpdateTimer: null,
      previousTimeLeft: '00:00',
      wasReservationFree: true,
      paidTimeStart: null,
      lastPaidMinute: -1, // Track the last minute we fetched cost for

      // Ongoing ride data
      showOngoingRidePopup: false,
      isOngoingRideExpanded: false,
      activeRental: null,
      currentRideCost: '0.00',
      rideTime: '00:00',
      rideTimer: null,
      rideStartTime: null,
      isBikeLocked: false,
      lastRideMinute: -1,
      originalZoneId: null, // Store the original zone where bike was rented

      // Payment data
      showPaymentPopup: false,
      isPaymentExpanded: false,
      finalRideCost: '0.00',
      finalRideTime: '00:00',
      costBreakdown: {
        showBreakdown: false,
        reservationCost: 0,
        rideCost: 0,
        reservationStartCost: 0 // Track cost at start of ride
      },

      // Toast notification system
      toastMessage: '',
      toastType: 'success', // 'success', 'error', 'warning', 'info'
      toastTimer: null,

      // Confirmation modal
      showConfirmModal: false,
      confirmModal: {
        title: '',
        message: '',
        confirmText: 'Confirm',
        action: null
      },

      // Problem report modal
      showProblemModal: false,
      problemTypes: [
        'Bike is damaged',
        'Bike won\'t unlock',
        'Bike won\'t lock',
        'Battery is dead',
        'Other issue'
      ],
    }
  },
  mounted() {
    this.initMap()
    this.createBikeIcon()
    this.checkAuthStatus()

    if (this.isLoggedIn) {
      this.fetchAndDisplayZones()
    }

    // Add global mouse event listeners
    document.addEventListener('mousemove', this.handleGlobalMouseMove)
    document.addEventListener('mouseup', this.handleGlobalMouseUp)

    this.checkForActiveReservation()
    this.checkForActiveRental()
  },
  beforeUnmount() {
    // Clean up global listeners
    document.removeEventListener('mousemove', this.handleGlobalMouseMove)
    document.removeEventListener('mouseup', this.handleGlobalMouseUp)

    this.clearTimers()
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
      this.showToast('Click on the map to start drawing a zone. Double-click to finish.', 'info')

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
          this.handleZoneClick(zone, availableBikes)
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

    async checkForActiveReservation() {
      if (!this.isLoggedIn) return

      try {
        const response = await api.getActiveReservation()
        if (response.data) {
          this.activeReservation = response.data
          this.showActiveReservationPopup = true
          this.startReservationTracking()
        }
      } catch (error) {
        if (error.response?.status !== 404) {
          console.error('Error checking active reservation:', error)
        }
      }
    },

    async checkForActiveRental() {
      if (!this.isLoggedIn) return

      try {
        const response = await api.getActiveRental()
        if (response.data) {
          this.activeRental = response.data

          // Store original zone ID from the rental data using startZoneId
          this.originalZoneId = this.activeRental.startZoneId

          // Set ride start time from rental data or use current time
          this.rideStartTime = this.activeRental.startTime ?
            new Date(this.activeRental.startTime) :
            new Date()

          // If there's a reservationId, fetch reservation cost for breakdown
          if (this.activeRental.reservationId) {
            await this.fetchReservationCostForBreakdown(this.activeRental.reservationId)
          }

          // Show ongoing ride popup
          this.showOngoingRidePopup = true
          this.startRideTracking()

          console.log('Active rental found:', this.activeRental)
          console.log('Original zone ID:', this.originalZoneId)
        }
      } catch (error) {
        if (error.response?.status !== 404) {
          console.error('Error checking active rental:', error)
        }
      }
    },

    async fetchReservationCostForBreakdown(reservationId) {
      try {
        // Get reservation cost for breakdown calculation
        const reservationCostResponse = await api.getReservationCost(reservationId)
        this.costBreakdown.reservationStartCost = reservationCostResponse.data

        console.log('Fetched reservation cost for breakdown:', this.costBreakdown.reservationStartCost)
      } catch (error) {
        console.error('Error fetching reservation cost for breakdown:', error)
        // If we can't get reservation cost, set to 0
        this.costBreakdown.reservationStartCost = 0
      }
    },

    handleZoneClick(zone, availableBikes) {
      // If user has an active rental, show the rental popup instead
      if (this.activeRental) {
        this.showOngoingRidePopup = true
        return
      }

      // If user has an active reservation, show the reservation popup instead
      if (this.activeReservation) {
        this.showActiveReservationPopup = true
        return
      }

      // Otherwise, show zone bikes as normal
      this.showZoneBikes(zone, availableBikes)
    },

    showZoneBikes(zone, availableBikes) {
      this.selectedZone = zone
      this.availableBikes = availableBikes
      this.showZonePopup = true
      this.isZonePopupExpanded = false
    },

    closeZonePopup() {
      this.showZonePopup = false
      this.selectedZone = null
      this.availableBikes = []
      this.isZonePopupExpanded = false
    },

    async startReservationTracking() {
      if (!this.activeReservation) return

      // Get initial status
      await this.updateReservationStatus()

      // Update countdown every second for smooth display
      this.reservationTimer = setInterval(() => {
        this.updateTimeLeft()
      }, 1000)

      // Update cost and check API status every 30 seconds
      this.costUpdateTimer = setInterval(async () => {
        if (!this.isReservationFree) {
          await this.updateCost()
        }
        // Refresh reservation status from API every 30 seconds
        await this.updateReservationStatus()
      }, 30000)
    },

    async updateReservationStatus() {
      if (!this.activeReservation) return

      try {
        // Get fresh reservation data
        const reservationResponse = await api.getActiveReservation()
        if (reservationResponse.data) {
          this.activeReservation = reservationResponse.data
        }

        // Check if reservation is still free
        const freeResponse = await api.checkReservationIsFree(this.activeReservation.id)
        this.isReservationFree = freeResponse.data

        // Update cost
        await this.updateCost()
      } catch (error) {
        console.error('Error updating reservation status:', error)
      }
    },

    async updateCost() {
      if (!this.activeReservation) return

      try {
        const costResponse = await api.getReservationCost(this.activeReservation.id)
        this.currentCost = costResponse.data.toFixed(2)
      } catch (error) {
        console.error('Error updating cost:', error)
      }
    },

    updateTimeLeft() {
      if (!this.activeReservation) return

      const now = new Date()
      const startTime = new Date(this.activeReservation.startTime)

      // Store previous values to detect transitions
      const previousFreeStatus = this.isReservationFree

      // If endTime is available, use it for countdown
      if (this.activeReservation.endTime) {
        const endTime = new Date(this.activeReservation.endTime)
        const timeLeftMs = endTime.getTime() - now.getTime()

        if (timeLeftMs <= 0) {
          // Transition to paid time
          if (this.isReservationFree) {
            this.isReservationFree = false
            this.paidTimeStart = new Date()
            this.lastPaidMinute = -1 // Reset minute tracker
          }

          // Count up from when paid time started
          if (this.paidTimeStart) {
            const paidElapsedMs = now.getTime() - this.paidTimeStart.getTime()
            const minutes = Math.floor(paidElapsedMs / (1000 * 60))
            const seconds = Math.floor((paidElapsedMs % (1000 * 60)) / 1000)
            this.timeLeft = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`

            // Check if we hit a new minute (xx:00) during paid time
            if (seconds === 0 && minutes > this.lastPaidMinute) {
              this.lastPaidMinute = minutes
              console.log(`Paid time hit ${minutes} minutes, fetching updated cost...`)
              this.updateCost()
            }
          } else {
            this.timeLeft = '00:00'
          }
        } else {
          const minutes = Math.floor(timeLeftMs / (1000 * 60))
          const seconds = Math.floor((timeLeftMs % (1000 * 60)) / 1000)
          this.timeLeft = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
        }
      } else {
        // If no endTime, calculate based on 10 minute reservation period
        const reservationDurationMs = 10 * 60 * 1000 // 10 minutes
        const elapsedMs = now.getTime() - startTime.getTime()
        const timeLeftMs = reservationDurationMs - elapsedMs

        if (timeLeftMs <= 0) {
          // Transition to paid time
          if (this.isReservationFree) {
            this.isReservationFree = false
            this.paidTimeStart = new Date(startTime.getTime() + reservationDurationMs)
            this.lastPaidMinute = -1 // Reset minute tracker
          }

          // Count up from when paid time started (10 minutes after start)
          const paidElapsedMs = now.getTime() - (startTime.getTime() + reservationDurationMs)
          const minutes = Math.floor(paidElapsedMs / (1000 * 60))
          const seconds = Math.floor((paidElapsedMs % (1000 * 60)) / 1000)
          this.timeLeft = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`

          // Check if we hit a new minute (xx:00) during paid time
          if (seconds === 0 && minutes > this.lastPaidMinute) {
            this.lastPaidMinute = minutes
            console.log(`Paid time hit ${minutes} minutes, fetching updated cost...`)
            this.updateCost()
          }
        } else {
          const minutes = Math.floor(timeLeftMs / (1000 * 60))
          const seconds = Math.floor((timeLeftMs % (1000 * 60)) / 1000)
          this.timeLeft = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
          this.isReservationFree = true
        }
      }

      // Detect transition from free to paid and fetch cost
      if (previousFreeStatus && !this.isReservationFree) {
        console.log('Reservation transitioned from free to paid, fetching cost...')
        this.updateCost()
      }
    },

    clearTimers() {
      if (this.reservationTimer) {
        clearInterval(this.reservationTimer)
        this.reservationTimer = null
      }
      if (this.costUpdateTimer) {
        clearInterval(this.costUpdateTimer)
        this.costUpdateTimer = null
      }
      if (this.rideTimer) {
        clearInterval(this.rideTimer)
        this.rideTimer = null
      }
    },

    // Toast notification methods
    showToast(message, type = 'success', duration = 4000) {
      this.toastMessage = message
      this.toastType = type

      if (this.toastTimer) {
        clearTimeout(this.toastTimer)
      }

      this.toastTimer = setTimeout(() => {
        this.closeToast()
      }, duration)
    },

    closeToast() {
      this.toastMessage = ''
      if (this.toastTimer) {
        clearTimeout(this.toastTimer)
        this.toastTimer = null
      }
    },

    // Confirmation modal methods
    showConfirmation(title, message, confirmText = 'Confirm') {
      return new Promise((resolve) => {
        this.confirmModal = {
          title,
          message,
          confirmText,
          action: resolve
        }
        this.showConfirmModal = true
      })
    },

    confirmAction() {
      if (this.confirmModal.action) {
        this.confirmModal.action(true)
      }
      this.closeConfirmModal()
    },

    closeConfirmModal() {
      if (this.confirmModal.action) {
        this.confirmModal.action(false)
      }
      this.showConfirmModal = false
      this.confirmModal = { title: '', message: '', confirmText: 'Confirm', action: null }
    },

    // Problem report modal methods
    openProblemModal() {
      this.showProblemModal = true
    },

    closeProblemModal() {
      this.showProblemModal = false
    },

    selectBike(bike) {
      // Check if user already has an active rental
      if (this.activeRental) {
        this.showToast('You already have an active bike rental. Please complete your current ride first.', 'warning')
        return
      }

      // Check if user already has an active reservation
      if (this.activeReservation) {
        this.showToast('You already have an active bike reservation. Please complete or cancel your current reservation first.', 'warning')
        return
      }

      this.selectedBike = bike
      this.showZonePopup = false
      this.showReservationPopup = true
      this.isReservationPopupExpanded = false
    },

    async beginBikeReservation() {
      if (this.selectedBike) {
        const bikeModel = this.selectedBike.model
        const bikeId = this.selectedBike.id

        try {
          const response = await api.getActiveReservation()
          if (response.data) {
            this.showToast('You already have an active reservation.', 'warning')
            this.closeReservationPopup()
            this.activeReservation = response.data
            this.showActiveReservationPopup = true
            this.startReservationTracking()
            return
          }
        } catch (error) {
          console.log('No active reservation found, proceeding with new reservation')
        }

        try {
          console.log('Creating reservation for bike:', bikeId)
          const reservationResponse = await api.createReservation(bikeId)
          console.log('Reservation response:', reservationResponse)
          this.activeReservation = reservationResponse.data
          this.closeReservationPopup()
          this.showActiveReservationPopup = true
          this.startReservationTracking()

          this.showToast(`Reservation created for bike: ${bikeModel}`, 'success')
        } catch (error) {
          console.error('Full error object:', error)

          let errorMessage = 'Failed to create reservation. '
          if (error.response?.data?.detail) {
            errorMessage += error.response.data.detail
          } else if (error.response?.data?.title) {
            errorMessage += error.response.data.title
          } else if (error.message) {
            errorMessage += error.message
          } else {
            errorMessage += 'Please try again.'
          }

          this.showToast(errorMessage, 'error')
        }
      }
    },

    async unlockBike() {
      if (!this.activeReservation) {
        this.showToast('No active reservation found.', 'error')
        return
      }

      try {
        console.log('Starting rental for reservation:', this.activeReservation.id)

        this.costBreakdown.reservationStartCost = parseFloat(this.currentCost)

        const rentalResponse = await api.startRental(this.activeReservation.id)
        console.log('Rental started:', rentalResponse)

        this.activeRental = rentalResponse.data
        this.rideStartTime = new Date()

        this.originalZoneId = this.activeRental.startZoneId ||
          this.activeReservation.zoneId ||
          (this.selectedZone ? this.selectedZone.id : null)

        this.activeReservation = null

        this.showActiveReservationPopup = false
        this.showOngoingRidePopup = true
        this.clearTimers()
        this.startRideTracking()

        console.log('Bike unlocked, original zone ID:', this.originalZoneId)
        console.log('Reservation cost at start:', this.costBreakdown.reservationStartCost)
        this.showToast('Bike unlocked successfully! Your ride has started.', 'success')

      } catch (error) {
        console.error('Error starting rental:', error)

        let errorMessage = 'Failed to unlock bike. '
        if (error.response?.data?.detail) {
          errorMessage += error.response.data.detail
        } else if (error.response?.data?.title) {
          errorMessage += error.response.data.title
        } else if (error.message) {
          errorMessage += error.message
        } else {
          errorMessage += 'Please try again.'
        }

        this.showToast(errorMessage, 'error')
      }
    },

    async updateRideCost() {
      if (!this.activeRental) return

      try {
        const costResponse = await api.getRentalCost(this.activeRental.id)
        this.currentRideCost = costResponse.data.toFixed(2)
      } catch (error) {
        console.error('Error updating ride cost:', error)
      }
    },

    updateRideTime() {
      if (!this.rideStartTime) return

      const now = new Date()
      const elapsedMs = now.getTime() - this.rideStartTime.getTime()

      const hours = Math.floor(elapsedMs / (1000 * 60 * 60))
      const minutes = Math.floor((elapsedMs % (1000 * 60 * 60)) / (1000 * 60))
      const seconds = Math.floor((elapsedMs % (1000 * 60)) / 1000)

      if (hours > 0) {
        this.rideTime = `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
      } else {
        this.rideTime = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
      }

      if (seconds === 0 && minutes > this.lastRideMinute) {
        this.lastRideMinute = minutes
        console.log(`Ride time hit ${minutes} minutes, fetching updated cost...`)
        this.updateRideCost()
      }
    },

    async lockBike() {
      if (!this.activeRental) return

      try {
        await api.lockBike(this.activeRental.id)
        this.isBikeLocked = true
        this.showToast('Bike locked successfully!', 'success')
      } catch (error) {
        console.error('Error locking bike:', error)
        this.showToast('Failed to lock bike. Please try again.', 'error')
      }
    },

    async unlockBikeRental() {
      if (!this.activeRental) return

      try {
        await api.unlockBike(this.activeRental.id)
        this.isBikeLocked = false
        this.showToast('Bike unlocked successfully!', 'success')
      } catch (error) {
        console.error('Error unlocking bike:', error)
        this.showToast('Failed to unlock bike. Please try again.', 'error')
      }
    },

    async finishRide() {
      if (!this.activeRental) {
        this.showToast('No active rental found.', 'error')
        return
      }

      if (!this.originalZoneId) {
        this.showToast('Error: Original zone information not found. Please contact support.', 'error')
        return
      }

      const confirmed = await this.showConfirmation(
        'Finish Ride',
        'Are you sure you want to finish your ride? This will end the rental and calculate the final cost.',
        'Finish Ride'
      )

      if (!confirmed) return

      try {
        console.log('Ending rental:', this.activeRental.id, 'with zone:', this.originalZoneId)
        const endResponse = await api.endRental(this.activeRental.id, this.originalZoneId)
        console.log('Rental ended:', endResponse)

        this.finalRideCost = this.currentRideCost
        this.finalRideTime = this.rideTime

        await this.calculateCostBreakdownFromRental()

        this.activeRental = null

        this.showOngoingRidePopup = false
        this.showPaymentPopup = true
        this.clearTimers()

        this.showToast('Ride finished! Please proceed with payment.', 'success')

      } catch (error) {
        console.error('Error ending rental:', error)

        let errorMessage = 'Failed to finish ride. '
        if (error.response?.data?.detail) {
          errorMessage += error.response.data.detail
        } else if (error.response?.data?.title) {
          errorMessage += error.response.data.title
        } else if (error.message) {
          errorMessage += error.message
        } else {
          errorMessage += 'Please try again.'
        }

        this.showToast(errorMessage, 'error')
      }
    },

    async calculateCostBreakdownFromRental() {
      const totalCost = parseFloat(this.finalRideCost)
      let reservationCost = 0

      if (this.activeRental && this.activeRental.reservationId) {
        try {
          const reservationCostResponse = await api.getReservationCost(this.activeRental.reservationId)
          reservationCost = reservationCostResponse.data

          console.log('Final reservation cost:', reservationCost)
        } catch (error) {
          console.error('Error fetching final reservation cost:', error)
          reservationCost = this.costBreakdown.reservationStartCost || 0
        }
      } else {
        reservationCost = this.costBreakdown.reservationStartCost || 0
      }

      const rideCost = Math.max(0, totalCost - reservationCost)

      this.costBreakdown = {
        showBreakdown: true,
        reservationCost: Math.max(0, reservationCost),
        rideCost: rideCost,
        reservationStartCost: reservationCost
      }

      console.log('Cost breakdown calculated from rental:', {
        total: totalCost,
        reservation: this.costBreakdown.reservationCost,
        ride: this.costBreakdown.rideCost,
        reservationId: this.activeRental?.reservationId
      })
    },

    calculateCostBreakdown() {
      const totalCost = parseFloat(this.finalRideCost)
      const reservationCost = this.costBreakdown.reservationStartCost
      const rideCost = totalCost - reservationCost

      this.costBreakdown = {
        showBreakdown: true,
        reservationCost: Math.max(0, reservationCost),
        rideCost: Math.max(0, rideCost),
        reservationStartCost: reservationCost
      }

      console.log('Cost breakdown calculated:', {
        total: totalCost,
        reservation: this.costBreakdown.reservationCost,
        ride: this.costBreakdown.rideCost
      })
    },

    togglePaymentExpanded() {
      this.isPaymentExpanded = !this.isPaymentExpanded
    },

    payForRide() {
      this.showToast(`Payment of ${this.finalRideCost}€ processed successfully! Thank you for using our service.`, 'success')
      this.closePaymentPopup()
    },

    goToUserPage() {
      this.$router.push('/user')
    },

    closeReservationPopup() {
      this.showReservationPopup = false
      this.selectedBike = null
      this.isReservationPopupExpanded = false
    },

    closeActiveReservationPopup() {
      this.showActiveReservationPopup = false
    },

    closeOngoingRidePopup() {
      this.showOngoingRidePopup = false
    },

    toggleZonePopupExpanded() {
      this.isZonePopupExpanded = !this.isZonePopupExpanded
    },

    toggleReservationPopupExpanded() {
      this.isReservationPopupExpanded = !this.isReservationPopupExpanded
    },

    toggleActiveReservationExpanded() {
      this.isActiveReservationExpanded = !this.isActiveReservationExpanded
    },

    toggleOngoingRideExpanded() {
      this.isOngoingRideExpanded = !this.isOngoingRideExpanded
    },

    handleTouchStart(event) {
      this.isDragging = true
      this.dragStartY = event.touches[0].clientY
      this.dragCurrentY = this.dragStartY
    },

    handleTouchMove(event) {
      if (!this.isDragging) return
      event.preventDefault()
      this.dragCurrentY = event.touches[0].clientY
    },

    handleTouchEnd() {
      if (!this.isDragging) return
      this.isDragging = false

      const dragDistance = this.dragStartY - this.dragCurrentY
      const threshold = this.dragThreshold

      if (Math.abs(dragDistance) > threshold) {
        if (dragDistance > 0) {
          // Dragged up - expand
          if (this.showZonePopup) this.isZonePopupExpanded = true
          if (this.showReservationPopup) this.isReservationPopupExpanded = true
          if (this.showActiveReservationPopup) this.isActiveReservationExpanded = true
          if (this.showOngoingRidePopup) this.isOngoingRideExpanded = true
          if (this.showPaymentPopup) this.isPaymentExpanded = true
        } else {
          // Dragged down - collapse
          if (this.showZonePopup) this.isZonePopupExpanded = false
          if (this.showReservationPopup) this.isReservationPopupExpanded = false
          if (this.showActiveReservationPopup) this.isActiveReservationExpanded = false
          if (this.showOngoingRidePopup) this.isOngoingRideExpanded = false
          if (this.showPaymentPopup) this.isPaymentExpanded = false
        }
      }

      this.resetDragState()
    },

    handleMouseDown(event) {
      this.isMouseDragging = true
      this.isDragging = true
      this.dragStartY = event.clientY
      this.dragCurrentY = this.dragStartY
      event.preventDefault()
    },

    handleGlobalMouseMove(event) {
      if (!this.isDragging || !this.isMouseDragging) return
      this.dragCurrentY = event.clientY
    },

    handleGlobalMouseUp() {
      if (!this.isDragging || !this.isMouseDragging) return
      this.isDragging = false
      this.isMouseDragging = false

      const dragDistance = this.dragStartY - this.dragCurrentY
      const threshold = this.dragThreshold

      if (Math.abs(dragDistance) > threshold) {
        if (dragDistance > 0) {
          // Dragged up - expand
          if (this.showZonePopup) this.isZonePopupExpanded = true
          if (this.showReservationPopup) this.isReservationPopupExpanded = true
          if (this.showActiveReservationPopup) this.isActiveReservationExpanded = true
          if (this.showOngoingRidePopup) this.isOngoingRideExpanded = true
          if (this.showPaymentPopup) this.isPaymentExpanded = true
        } else {
          // Dragged down - collapse
          if (this.showZonePopup) this.isZonePopupExpanded = false
          if (this.showReservationPopup) this.isReservationPopupExpanded = false
          if (this.showActiveReservationPopup) this.isActiveReservationExpanded = false
          if (this.showOngoingRidePopup) this.isOngoingRideExpanded = false
          if (this.showPaymentPopup) this.isPaymentExpanded = false
        }
      }

      this.resetDragState()
    },

    resetDragState() {
      this.isDragging = false
      this.isMouseDragging = false
      this.dragStartY = 0
      this.dragCurrentY = 0
    },
  },
}
</script>

<style scoped>
.map-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  position: relative;
  overflow: hidden;
}

.floating-user-icon {
  position: fixed;
  top: 20px;
  right: 20px;
  width: 50px;
  height: 50px;
  background-color: #009688;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 1000;
  transition: all 0.3s ease;
}

.floating-user-icon:hover {
  background-color: #00796b;
  transform: scale(1.1);
}

.floating-user-icon:active {
  transform: scale(0.95);
}

.desktop-only {
  display: none;
}

.map-view {
  width: 100%;
  height: 100vh;
  flex: 1;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
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

.bottom-popup {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  /* background: rgba(0, 0, 0, 0.5); */
  z-index: 10000;
  display: flex;
  align-items: flex-end;
  animation: fadeIn 0.3s ease-out;
}

.bottom-popup.full-screen {
  align-items: stretch;
}

.bottom-popup-content {
  background: white;
  width: 100%;
  border-radius: 20px 20px 0 0;
  padding: 20px;
  max-height: 70vh;
  overflow-y: auto;
  animation: slideUp 0.3s ease-out;
  transition: all 0.3s ease-out;
}

.bottom-popup-content.expanded {
  height: 100vh;
  max-height: 100vh;
  border-radius: 0;
}

.popup-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  border-bottom: 1px solid #eee;
  padding-bottom: 12px;
  position: relative;
}

.drag-handle {
  position: absolute;
  top: -15px;
  left: 50%;
  transform: translateX(-50%);
  width: 60px;
  height: 30px;
  cursor: grab;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1;
  padding: 10px;
}

.drag-handle:active {
  cursor: grabbing;
}

.drag-indicator {
  width: 40px;
  height: 4px;
  background-color: #ccc;
  border-radius: 2px;
  transition: background-color 0.2s, width 0.2s;
}

.drag-handle:hover .drag-indicator {
  background-color: #999;
  width: 50px;
}

.drag-handle:active .drag-indicator {
  background-color: #666;
}

.popup-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  color: #666;
  cursor: pointer;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.close-btn:hover {
  background-color: #f0f0f0;
}

.zone-subtitle {
  margin: 0 0 8px 0;
  color: #666;
  font-size: 14px;
}

.bikes-count {
  margin: 0 0 20px 0;
  font-weight: 600;
  color: #00897b;
  font-size: 16px;
}

.bikes-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.bike-card {
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid #e9ecef;
}

.bike-card:hover {
  background-color: #e3f2fd;
  border-color: #2196f3;
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.bike-card-content {
  margin-bottom: 16px;
}

.bike-name {
  margin: 0 0 12px 0;
  font-size: 18px;
  font-weight: 600;
  color: #333;
  text-align: center;
}

.bike-details {
  background-color: white;
  border-radius: 8px;
  padding: 12px;
}

.bike-detail {
  display: flex;
  justify-content: space-between;
  font-size: 16px;
}

.bike-detail span:first-child {
  color: #666;
}

.price {
  font-weight: 600;
  color: #00897b;
}

.select-bike-btn {
  background-color: #00897b;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 12px 20px;
  width: 100%;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.select-bike-btn:hover {
  background-color: #00695c;
}

.select-bike-btn .icon {
  margin-right: 8px;
  font-size: 14px;
}

.no-bikes {
  margin: 20px 0;
}

.no-bikes-content {
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 40px 20px;
  text-align: center;
}

.no-bikes-content p {
  margin: 0;
  color: #666;
  font-style: italic;
}

.bike-model {
  font-weight: 600;
  font-size: 18px;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

.reservation-details {
  margin-bottom: 30px;
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
}

.reservation-detail {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  font-size: 16px;
}

.reservation-detail:last-child {
  margin-bottom: 0;
}

.reservation-detail span:first-child {
  color: #666;
}

.reservation-detail span:last-child {
  font-weight: 600;
  color: #333;
}

.begin-reservation-btn {
  background-color: #00897b;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.begin-reservation-btn:hover {
  background-color: #00695c;
}

.begin-reservation-btn .icon {
  margin-right: 8px;
  font-size: 16px;
}

.reservation-status {
  margin-bottom: 20px;
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
}

.status-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  font-size: 16px;
}

.status-item:last-child {
  margin-bottom: 0;
}

.status-item span:first-child {
  color: #666;
}

.cost {
  font-weight: 600;
  color: #e91e63;
  font-size: 18px;
}

.time-left {
  font-weight: 600;
  color: #00897b;
  font-size: 18px;
}

.paid-status {
  font-weight: 600;
  color: #ff9800;
}

.reservation-info {
  background-color: #e3f2fd;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 20px;
}

.info-text {
  margin: 0;
  color: #1976d2;
  font-size: 14px;
  text-align: center;
}

.action-buttons {
  margin: 20px 0;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.unlock-bike-btn {
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.unlock-bike-btn:hover {
  background-color: #45a049;
}

.cancel-reservation-btn {
  background-color: #f44336;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.cancel-reservation-btn:hover {
  background-color: #d32f2f;
}

.unlock-icon,
.cancel-icon {
  margin-right: 8px;
  font-size: 20px;
}

.lock-status {
  font-weight: 600;
  font-size: 16px;
}

.lock-status.locked {
  color: #f44336;
}

.lock-status.unlocked {
  color: #4caf50;
}

.lock-bike-btn {
  background-color: #ff9800;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.lock-bike-btn:hover {
  background-color: #f57c00;
}

.lock-icon {
  margin-right: 8px;
  font-size: 20px;
}

.finish-ride-btn {
  background-color: #ff5722;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.finish-ride-btn:hover {
  background-color: #e64a19;
}

.stop-icon {
  margin-right: 8px;
  font-size: 20px;
}

.pay-btn {
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.pay-btn:hover {
  background-color: #45a049;
}

.pay-icon {
  margin-right: 8px;
  font-size: 20px;
}

.ride-status {
  margin-bottom: 20px;
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
}

.payment-status {
  margin-bottom: 20px;
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
}

.cost-breakdown {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #e9ecef;
}

.breakdown-header {
  font-weight: 600;
  font-size: 14px;
  color: #666;
  margin-bottom: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.breakdown-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.breakdown-item:last-child {
  margin-bottom: 0;
}

.breakdown-item span:first-child {
  color: #666;
}

.breakdown-cost {
  font-weight: 500;
  color: #333;
}

.breakdown-item.total-line {
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #dee2e6;
  font-weight: 600;
  font-size: 16px;
}

.breakdown-total {
  color: #e91e63;
  font-weight: 600;
}

.report-problem-link {
  color: #f44336;
  text-align: center;
  font-size: 16px;
  cursor: pointer;
  text-decoration: underline;
  transition: color 0.2s;
}

.report-problem-link:hover {
  color: #d32f2f;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }

  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    transform: translateY(100%);
  }

  to {
    transform: translateY(0);
  }
}

@keyframes slideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }

  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes scaleIn {
  from {
    transform: scale(0.9);
    opacity: 0;
  }

  to {
    transform: scale(1);
    opacity: 1;
  }
}

@media (min-width: 1024px) {
  .map-container {
    height: auto;
    overflow: visible;
  }

  .desktop-only {
    display: block;
  }

  .map-view {
    position: relative;
    height: 500px;
    margin-bottom: 20px;
    top: auto;
    left: auto;
    right: auto;
    bottom: auto;
  }
}

/* Toast Notifications */
.toast-notification {
  position: fixed;
  top: 80px;
  right: 20px;
  z-index: 10001;
  max-width: 300px;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  animation: slideInRight 0.3s ease-out;
}

.toast-notification.success {
  background-color: #4caf50;
  color: white;
}

.toast-notification.error {
  background-color: #f44336;
  color: white;
}

.toast-notification.warning {
  background-color: #ff9800;
  color: white;
}

.toast-notification.info {
  background-color: #2196f3;
  color: white;
}

.toast-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px;
  gap: 12px;
}

.toast-close {
  background: none;
  border: none;
  color: inherit;
  font-size: 20px;
  cursor: pointer;
  padding: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.toast-close:hover {
  background-color: rgba(255, 255, 255, 0.2);
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 10002;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: fadeIn 0.3s ease-out;
}

.confirmation-modal,
.problem-modal {
  background: white;
  border-radius: 12px;
  padding: 24px;
  max-width: 400px;
  width: 90%;
  animation: scaleIn 0.3s ease-out;
}

.confirmation-modal h3,
.problem-modal h3 {
  margin: 0 0 16px 0;
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

.confirmation-modal p {
  margin: 0 0 24px 0;
  color: #666;
  line-height: 1.5;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

.cancel-btn-modal,
.confirm-btn-modal {
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.cancel-btn-modal {
  background-color: #f5f5f5;
  color: #666;
}

.cancel-btn-modal:hover {
  background-color: #e0e0e0;
}

.confirm-btn-modal {
  background-color: #f44336;
  color: white;
}

.confirm-btn-modal:hover {
  background-color: #d32f2f;
}

/* Problem Modal */
.problem-options {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 24px;
}

.problem-option {
  padding: 12px 16px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: white;
  color: #333;
  text-align: left;
  cursor: pointer;
  transition: all 0.2s;
}

.problem-option:hover {
  background-color: #f5f5f5;
  border-color: #2196f3;
}
</style>
