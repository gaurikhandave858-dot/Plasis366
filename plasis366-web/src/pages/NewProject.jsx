import { useEffect, useState } from 'react'
import { Link, useLocation, useNavigate, useParams } from 'react-router-dom'
import api from '../api'
import AttachmentsSection from '../components/AttachmentsSection'
import {
  createProject,
  getProject,
  updateProject,
  listAttachments,
  removeAttachment,
  uploadAttachments,
  openAttachment,
  submitProject,
} from '../services/projectService'
import { validateProject } from '../validation'
import './dashboard.css'
import './newproject.css'


const PROPERTY_TYPES = ['Flat', 'Villa', 'Bungalow', 'Office', 'Shop', 'Other']
const ROOM_TYPES = ['Living room', 'Bedroom', 'Kitchen', 'Bathroom', 'Dining room', 'Home office', 'Kids room', 'Balcony', 'Other']
const STYLES = ['Modern', 'Minimal', 'Contemporary', 'Classic', 'Scandinavian', 'Industrial', 'Traditional', 'Bohemian']

// Each level depends on the one above it
const LEVELS = [
  { key: 'regionId', label: 'Region', url: '/MasterData/regions' },
  { key: 'countryId', label: 'Country', url: '/MasterData/countries', param: 'regionId' },
  { key: 'stateId', label: 'State', url: '/MasterData/states', param: 'countryId' },
  { key: 'districtId', label: 'District', url: '/MasterData/districts', param: 'stateId' },
  { key: 'talukaId', label: 'Taluka', url: '/MasterData/talukas', param: 'districtId' },
  { key: 'cityId', label: 'City', url: '/MasterData/cities', param: 'talukaId' },
]

const emptyLoc = { regionId: '', countryId: '', stateId: '', districtId: '', talukaId: '', cityId: '' }
const emptyOptions = { regionId: [], countryId: [], stateId: [], districtId: [], talukaId: [], cityId: [] }
const emptyForm = {
  projectName: '', description: '', budgetMin: '', budgetMax: '', expectedStartDate: '',
  propertyType: 'Flat', propertyName: '', address: '', totalArea: '', numberOfFloors: '',
  preferredStyle: '', preferredColors: '', furnitureRequirements: '', storageRequirements: '',
  lightingRequirements: '', specialRequirements: '', additionalNotes: '',
}

const newRoom = (roomType = 'Living room') => ({
  roomId: null, roomType, roomName: '', length: '', width: '', height: '',
})

const num = (v) => (v === '' || v == null ? null : Number(v))
const text = (v) => (v === '' ? null : v)
const str = (v) => (v == null ? '' : String(v))

// Loads the dropdown lists for every level that already has a selected parent.
// For a new project that is just the regions. For an edit it fills in the saved location.
async function loadLocationOptions(locValues) {
  const result = { ...emptyOptions }
  for (let i = 0; i < LEVELS.length; i++) {
    const level = LEVELS[i]
    if (i === 0) {
      result[level.key] = (await api.get(level.url)).data
      continue
    }
    const parent = locValues[LEVELS[i - 1].key]
    if (!parent) break
    result[level.key] = (await api.get(level.url, { params: { [level.param]: parent } })).data
  }
  return result
}

function ProjectForm({ id }) {
  const navigate = useNavigate()
  const location = useLocation()
  const isEdit = Boolean(id)

  const [form, setForm] = useState(emptyForm)
  const [loc, setLoc] = useState(emptyLoc)
  const [options, setOptions] = useState(emptyOptions)
  const [rooms, setRooms] = useState([newRoom()])
  const [existing, setExisting] = useState([])   // files already saved on the server
  const [pending, setPending] = useState([])     // files chosen, uploaded when saving
  const [meta, setMeta] = useState(null)         // { status, projectCode } when editing
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState(location.state?.notice || '')
  const [saving, setSaving] = useState(false)

  // Submitted and later projects are shown read-only
  const readOnly = isEdit && meta != null && meta.status !== 'Draft'

  useEffect(() => {
    let cancelled = false

    async function init() {
      try {
        let locValues = emptyLoc

        if (isEdit) {
          const p = await getProject(id)
          if (cancelled) return

          locValues = {
            regionId: str(p.regionId),
            countryId: str(p.countryId),
            stateId: str(p.stateId),
            districtId: str(p.districtId),
            talukaId: str(p.talukaId),
            cityId: str(p.cityId),
          }
          const r = p.requirements || {}

          setForm({
            projectName: str(p.projectName),
            description: str(p.description),
            budgetMin: str(p.budgetMin),
            budgetMax: str(p.budgetMax),
            expectedStartDate: str(p.expectedStartDate).slice(0, 10),
            propertyType: p.propertyType || 'Flat',
            propertyName: str(p.propertyName),
            address: str(p.address),
            totalArea: str(p.totalArea),
            numberOfFloors: str(p.numberOfFloors),
            preferredStyle: str(r.preferredStyle),
            preferredColors: str(r.preferredColors),
            furnitureRequirements: str(r.furnitureRequirements),
            storageRequirements: str(r.storageRequirements),
            lightingRequirements: str(r.lightingRequirements),
            specialRequirements: str(r.specialRequirements),
            additionalNotes: str(r.additionalNotes),
          })
          setLoc(locValues)
          setRooms(
            p.rooms.length
              ? p.rooms.map((x) => ({
                  roomId: x.roomId,
                  roomType: x.roomType,
                  roomName: str(x.roomName),
                  length: str(x.length),
                  width: str(x.width),
                  height: str(x.height),
                }))
              : [newRoom()]
          )
          setMeta({ status: p.status, projectCode: p.projectCode })
          setExisting(await listAttachments(id))
        }

        const opts = await loadLocationOptions(locValues)
        if (!cancelled) setOptions(opts)
      } catch (err) {
        if (!cancelled) {
          setError(
            err.response?.status === 404
              ? 'Project not found.'
              : 'Could not load the page. Is the API running?'
          )
        }
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    init()
    return () => { cancelled = true }
  }, [id, isEdit])

  const set = (e) => setForm({ ...form, [e.target.name]: e.target.value })
  const setRoom = (i, e) =>
    setRooms(rooms.map((r, idx) => (idx === i ? { ...r, [e.target.name]: e.target.value } : r)))
  const addRoom = () => setRooms([...rooms, newRoom('Bedroom')])
  const removeRoom = (i) => setRooms(rooms.filter((_, idx) => idx !== i))

  // When one dropdown changes: clear everything below it, then load the next list
  const pick = async (i, value) => {
    const clearedLoc = {}
    const clearedOptions = {}
    LEVELS.slice(i + 1).forEach((l) => {
      clearedLoc[l.key] = ''
      clearedOptions[l.key] = []
    })
    setLoc((l) => ({ ...l, [LEVELS[i].key]: value, ...clearedLoc }))
    setOptions((o) => ({ ...o, ...clearedOptions }))

    const next = LEVELS[i + 1]
    if (value && next) {
      try {
        const res = await api.get(next.url, { params: { [next.param]: value } })
        setOptions((o) => ({ ...o, [next.key]: res.data }))
      } catch {
        setError('Could not load the location lists.')
      }
    }
  }

  const handleOpen = async (a) => {
    try {
      await openAttachment(id, a.attachmentId)
    } catch {
      setError('Could not open the file.')
    }
  }

  const handleRemoveExisting = async (a) => {
    if (!window.confirm(`Remove "${a.fileName}"?`)) return
    try {
      await removeAttachment(id, a.attachmentId)
      setExisting((list) => list.filter((x) => x.attachmentId !== a.attachmentId))
    } catch (err) {
      setError(err.response?.data?.message || 'Could not remove the file.')
    }
  }

  const submit = async (e) => {
    e.preventDefault()
    setError('')
    setNotice('')

    // Which button was pressed: "Save" (draft) or "Submit to designer"
    const andSubmit = e.nativeEvent.submitter?.dataset.action === 'submit'

    const problem = validateProject(form, rooms)
    if (problem) return setError(problem)

    if (
      andSubmit &&
      !window.confirm('Submit this project to our designers? You will not be able to edit it afterwards.')
    ) {
      return
    }

    setSaving(true)
    try {
      const payload = {
        projectName: form.projectName,
        description: text(form.description),
        budgetMin: num(form.budgetMin),
        budgetMax: num(form.budgetMax),
        expectedStartDate: text(form.expectedStartDate),
        propertyType: form.propertyType,
        propertyName: text(form.propertyName),
        address: text(form.address),
        regionId: num(loc.regionId),
        countryId: num(loc.countryId),
        stateId: num(loc.stateId),
        districtId: num(loc.districtId),
        talukaId: num(loc.talukaId),
        cityId: num(loc.cityId),
        totalArea: num(form.totalArea),
        numberOfFloors: num(form.numberOfFloors),
        rooms: rooms.map((r) => ({
          roomId: r.roomId,             // lets the API tell an existing room from a new one
          roomType: r.roomType,
          roomName: text(r.roomName),
          length: num(r.length),
          width: num(r.width),
          height: num(r.height),        // not on the form, but sent back so it is never erased
        })),
        requirements: {
          preferredStyle: text(form.preferredStyle),
          preferredColors: text(form.preferredColors),
          furnitureRequirements: text(form.furnitureRequirements),
          storageRequirements: text(form.storageRequirements),
          lightingRequirements: text(form.lightingRequirements),
          specialRequirements: text(form.specialRequirements),
          additionalNotes: text(form.additionalNotes),
        },
      }

      let projectId = id
      if (isEdit) {
        await updateProject(id, payload)
      } else {
        projectId = (await createProject(payload)).projectId
      }

      // Files upload after the project exists, because they need its id
      if (pending.length > 0) {
        try {
          await uploadAttachments(projectId, pending)
        } catch (uploadErr) {
          const reason = uploadErr.response?.data?.message
          const message =
            `Project saved, but some files could not be uploaded${reason ? `: ${reason}` : '.'} ` +
            'Please check the list below and add any missing files.'

          if (isEdit) {
            setPending([])
            setExisting(await listAttachments(id))
            setError(message)
          } else {
            navigate(`/projects/${projectId}/edit`, { replace: true, state: { notice: message } })
          }
          return
        }
      }

      // Final step: move the project from Draft to Submitted
      if (andSubmit) {
        try {
          await submitProject(projectId)
        } catch (submitErr) {
          const message =
            'Project saved as a draft, but it could not be submitted: ' +
            (submitErr.response?.data?.message || 'please try again.')
          if (isEdit) {
            setError(message)
          } else {
            navigate(`/projects/${projectId}/edit`, { replace: true, state: { notice: message } })
          }
          return
        }
      }

      navigate('/dashboard')
    } catch (err) {
      setError(
        err.response?.data?.message ||
        err.response?.data?.title ||
        'Could not save the project. Is the API running?'
      )
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return <div className="pd"><p className="sub">Loading…</p></div>
  }

  if (isEdit && !meta) {
    return (
      <div className="pd">
        <p className="err">{error || 'Project not found.'}</p>
        <Link to="/dashboard">Back to dashboard</Link>
      </div>
    )
  }

  const heroSmall = readOnly ? 'View only' : isEdit ? 'Keep refining' : "Let's begin"
  const heroTitle = readOnly ? 'Project details' : isEdit ? 'Edit project' : 'New project'
  const heroSub = readOnly
    ? `${meta.projectCode} · ${meta.status}. This project can no longer be edited.`
    : isEdit
      ? `${meta.projectCode} · Draft. Changes are saved when you press Save.`
      : 'Tell us about your space. You can save it as a draft and submit it later.'

  return (
    <form className="pd" onSubmit={submit}>
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">{heroSmall}</p>
          <h1>{heroTitle}</h1>
          <p className="pd-hero-sub">{heroSub}</p>
        </div>
      </section>

      {notice && <p className="np-notice">{notice}</p>}

      <fieldset className="np-fieldset" disabled={readOnly}>
        <section className="pd-card">
          <div className="pd-card-head"><h2>Project</h2></div>
          <div className="np-grid">
            <div className="np-field full">
              <label htmlFor="projectName">Project name</label>
              <input id="projectName" name="projectName" value={form.projectName} onChange={set} placeholder="e.g. Modern 2BHK Living Room" required />
            </div>
            <div className="np-field full">
              <label htmlFor="description">Description</label>
              <textarea id="description" name="description" value={form.description} onChange={set} placeholder="What would you like us to design?" />
            </div>
            <div className="np-field">
              <label htmlFor="budgetMin">Budget from (₹)</label>
              <input id="budgetMin" name="budgetMin" type="number" min="0" value={form.budgetMin} onChange={set} />
            </div>
            <div className="np-field">
              <label htmlFor="budgetMax">Budget up to (₹)</label>
              <input id="budgetMax" name="budgetMax" type="number" min="0" value={form.budgetMax} onChange={set} />
            </div>
            <div className="np-field">
              <label htmlFor="expectedStartDate">Expected start date</label>
              <input id="expectedStartDate" name="expectedStartDate" type="date" value={form.expectedStartDate} onChange={set} />
            </div>
          </div>
        </section>

        <section className="pd-card">
          <div className="pd-card-head"><h2>Property</h2></div>
          <div className="np-grid">
            <div className="np-field">
              <label htmlFor="propertyType">Property type</label>
              <select id="propertyType" name="propertyType" value={form.propertyType} onChange={set}>
                {PROPERTY_TYPES.map((t) => <option key={t}>{t}</option>)}
              </select>
            </div>
            <div className="np-field">
              <label htmlFor="propertyName">Property name</label>
              <input id="propertyName" name="propertyName" value={form.propertyName} onChange={set} placeholder="Optional" />
            </div>

            {LEVELS.map((level, i) => (
              <div className="np-field" key={level.key}>
                <label htmlFor={level.key}>{level.label}</label>
                <select
                  id={level.key}
                  value={loc[level.key]}
                  onChange={(e) => pick(i, e.target.value)}
                  disabled={i > 0 && !loc[LEVELS[i - 1].key]}
                >
                  <option value="">Select {level.label.toLowerCase()}</option>
                  {options[level.key].map((o) => (
                    <option key={o.id} value={o.id}>{o.name}</option>
                  ))}
                </select>
              </div>
            ))}

            <div className="np-field full">
              <label htmlFor="address">Area / street</label>
              <input id="address" name="address" value={form.address} onChange={set} placeholder="Building, street, landmark" />
            </div>
            <div className="np-field">
              <label htmlFor="totalArea">Total area (sq ft)</label>
              <input id="totalArea" name="totalArea" type="number" min="0" value={form.totalArea} onChange={set} />
            </div>
            <div className="np-field">
              <label htmlFor="numberOfFloors">Floors</label>
              <input id="numberOfFloors" name="numberOfFloors" type="number" min="0" value={form.numberOfFloors} onChange={set} />
            </div>
          </div>
        </section>

        <section className="pd-card">
          <div className="pd-card-head"><h2>Rooms</h2></div>
          {rooms.map((r, i) => (
            <div className="np-room" key={r.roomId ?? `new-${i}`}>
              <div className="np-field">
                <label>Room type</label>
                <select name="roomType" value={r.roomType} onChange={(e) => setRoom(i, e)}>
                  {ROOM_TYPES.map((t) => <option key={t}>{t}</option>)}
                </select>
              </div>
              <div className="np-field">
                <label>Name</label>
                <input name="roomName" value={r.roomName} onChange={(e) => setRoom(i, e)} placeholder="Optional" />
              </div>
              <div className="np-field">
                <label>Length (ft)</label>
                <input name="length" type="number" min="0" value={r.length} onChange={(e) => setRoom(i, e)} />
              </div>
              <div className="np-field">
                <label>Width (ft)</label>
                <input name="width" type="number" min="0" value={r.width} onChange={(e) => setRoom(i, e)} />
              </div>
              <button type="button" className="np-remove" onClick={() => removeRoom(i)} aria-label="Remove room">✕</button>
            </div>
          ))}
          <button type="button" className="np-add" onClick={addRoom}>+ Add another room</button>
        </section>

        <section className="pd-card">
          <div className="pd-card-head"><h2>Design requirements</h2></div>
          <div className="np-grid">
            <div className="np-field">
              <label htmlFor="preferredStyle">Preferred style</label>
              <select id="preferredStyle" name="preferredStyle" value={form.preferredStyle} onChange={set}>
                <option value="">No preference</option>
                {STYLES.map((s) => <option key={s}>{s}</option>)}
              </select>
            </div>
            <div className="np-field">
              <label htmlFor="preferredColors">Preferred colors</label>
              <input id="preferredColors" name="preferredColors" value={form.preferredColors} onChange={set} placeholder="e.g. warm beige, olive green" />
            </div>
            <div className="np-field full">
              <label htmlFor="furnitureRequirements">Furniture</label>
              <textarea id="furnitureRequirements" name="furnitureRequirements" value={form.furnitureRequirements} onChange={set} placeholder="Sofa, TV unit, study table…" />
            </div>
            <div className="np-field">
              <label htmlFor="storageRequirements">Storage</label>
              <textarea id="storageRequirements" name="storageRequirements" value={form.storageRequirements} onChange={set} />
            </div>
            <div className="np-field">
              <label htmlFor="lightingRequirements">Lighting</label>
              <textarea id="lightingRequirements" name="lightingRequirements" value={form.lightingRequirements} onChange={set} />
            </div>
            <div className="np-field full">
              <label htmlFor="specialRequirements">Special requirements</label>
              <textarea id="specialRequirements" name="specialRequirements" value={form.specialRequirements} onChange={set} />
            </div>
            <div className="np-field full">
              <label htmlFor="additionalNotes">Anything else?</label>
              <textarea id="additionalNotes" name="additionalNotes" value={form.additionalNotes} onChange={set} />
            </div>
          </div>
        </section>

        <section className="pd-card">
          <div className="pd-card-head"><h2>Attachments</h2></div>
          <AttachmentsSection
            existing={existing}
            pending={pending}
            onPendingChange={setPending}
            onRemoveExisting={handleRemoveExisting}
            onOpen={handleOpen}
            readOnly={readOnly}
          />
        </section>
      </fieldset>

      {error && <p className="err" style={{ margin: 0 }}>{error}</p>}

      <div className="np-actions">
        {!readOnly && (
          <>
            <button className="btn-c np-secondary" type="submit" data-action="save" disabled={saving}>
              {saving ? 'Saving…' : isEdit ? 'Save changes' : 'Save as draft'}
            </button>
            <button className="btn-c" type="submit" data-action="submit" disabled={saving}>
              {saving ? 'Please wait…' : 'Submit to designer'}
            </button>
          </>
        )}
        <Link to="/dashboard" className="np-cancel">{readOnly ? 'Back to dashboard' : 'Cancel'}</Link>
      </div>
    </form>
  )
}

// The key makes React start with a clean form whenever the project id changes
function NewProject() {
  const { id } = useParams()
  return <ProjectForm key={id ?? 'new'} id={id} />
}

export default NewProject