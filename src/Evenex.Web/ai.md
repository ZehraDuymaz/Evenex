# Evenex Frontend - AI Development Guidelines

## Tech Stack & Rules
* Framework: Angular (TypeScript).
* Styling: CSS. Implement Dark/Light mode using white/slate for light, deep black for dark, and vibrant purple for primary accents.
* Boundary: Generate ONLY the static UI/UX and Angular component structures. The human engineer will handle the complex HTTP integration in the Services.
* Create components for every reusable UI thing. 

## Organizer (Organizatör) Scope
* Venues: UI to create and select physical venues (Mekan).
* Venue Sections: UI to define seating arrangements and capacities (Koltuk düzeni, kontenjan).
* Events: Forms to define event properties (Tarih, saat, detay, fiyat, bilet açılış tarihi).
* Rules: UI to set dynamic pricing/discounts (öğrenci indirimi) and publish the event.

## Attendee (Kullanıcı) Scope
* Discovery: Layouts to list, categorize, and view event details.
* Selection: UI to view seating maps and select seats or ticket quantities.
* Checkout: Implement a strict visual 5-minute countdown timer locking the tickets.
* Fulfillment: Screens for payment confirmation and generating the final digital ticket output.