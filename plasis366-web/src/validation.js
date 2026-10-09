// src/validation.js
// Checks the New/Edit Project form before it is sent to the API.
// Returns '' when everything is fine, or the first error message.

const has = (v) => v !== '' && v != null

// field name in the form -> [label shown in the message, max characters]
const TEXT_LIMITS = {
  description: ['Description', 2000],
  propertyName: ['Property name', 150],
  address: ['Address', 500],
  preferredColors: ['Preferred colors', 200],
  furnitureRequirements: ['Furniture', 1000],
  storageRequirements: ['Storage', 1000],
  lightingRequirements: ['Lighting', 1000],
  specialRequirements: ['Special requirements', 1000],
  additionalNotes: ['Additional notes', 2000],
}

export function validateProject(form, rooms) {
  const name = (form.projectName ?? '').trim()
  if (name.length < 3) return 'Project name must be at least 3 characters.'
  if (name.length > 200) return 'Project name can be at most 200 characters.'

  for (const [key, [label, max]] of Object.entries(TEXT_LIMITS)) {
    if (String(form[key] ?? '').length > max) {
      return `${label} can be at most ${max} characters.`
    }
  }

  if (has(form.budgetMin) && Number(form.budgetMin) < 0) return 'Minimum budget cannot be negative.'
  if (has(form.budgetMax) && Number(form.budgetMax) < 0) return 'Maximum budget cannot be negative.'
  if (has(form.budgetMin) && has(form.budgetMax) && Number(form.budgetMax) < Number(form.budgetMin)) {
    return 'Maximum budget cannot be less than minimum budget.'
  }

  if (has(form.totalArea) && !(Number(form.totalArea) >= 1 && Number(form.totalArea) <= 1000000)) {
    return 'Total area must be between 1 and 1,000,000 sq ft.'
  }
  if (has(form.numberOfFloors)) {
    const f = Number(form.numberOfFloors)
    if (!Number.isInteger(f) || f < 0 || f > 200) return 'Number of floors must be a whole number from 0 to 200.'
  }

  if (rooms.length > 50) return 'A project can have at most 50 rooms.'
  for (let i = 0; i < rooms.length; i++) {
    const r = rooms[i]
    const n = i + 1
    if (String(r.roomName ?? '').length > 100) return `Room ${n}: name can be at most 100 characters.`
    for (const field of ['length', 'width']) {
      if (has(r[field]) && !(Number(r[field]) >= 0.01 && Number(r[field]) <= 1000)) {
        return `Room ${n}: ${field} must be between 0.01 and 1000 ft.`
      }
    }
  }

  return ''
}