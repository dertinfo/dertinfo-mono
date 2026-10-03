import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const configDir = path.dirname(fileURLToPath(import.meta.url));
const fixturesDir = path.resolve(configDir, '../fixtures');

export const groups = JSON.parse(fs.readFileSync(path.join(configDir, 'groups.json'), 'utf8'));
export const events = JSON.parse(fs.readFileSync(path.join(configDir, 'events.json'), 'utf8'));

export const group = groups[0];
export const event = events[0];

export const GROUP_NAME = group.GroupName;
export const EVENT_NAME = event.Name;

const audienceLabels = {
  INDIVIDUAL: 'Individual',
  TEAM: 'Team',
};

export function fixtureImage(record) {
  const fileName = record && record.Image;
  if (!fileName) throw new Error('Config record has no Image');
  const filePath = path.join(fixturesDir, fileName);
  if (!fs.existsSync(filePath)) throw new Error(`Smoke image is missing: ${filePath}`);
  return filePath;
}

export function formDate(isoDate) {
  const [year, month, day] = String(isoDate).split('-');
  if (!year || !month || !day) throw new Error(`Expected an ISO date, received ${isoDate}`);
  return `${day}/${month}/${year}`;
}

export function audienceLabel(activity) {
  const label = audienceLabels[activity.AudienceTypeId];
  if (!label) throw new Error(`Unknown AudienceTypeId ${activity.AudienceTypeId}`);
  return label;
}

export function member(memberType, record = group) {
  const found = record.GroupMembers.find((person) => person.MemberType === memberType);
  if (!found) throw new Error(`${record.GroupName} has no ${memberType}`);
  return found;
}

export function team(record = group) {
  const found = record.Teams[0];
  if (!found) throw new Error(`${record.GroupName} has no team`);
  return found;
}

export function activitiesByAudience(record, audienceType) {
  return record.Activities.filter((activity) => activity.AudienceTypeId === audienceType);
}

export function defaultActivity(record = event) {
  const found = record.Activities.find((activity) => activity.IsDefault && activity.AudienceTypeId === 'INDIVIDUAL');
  if (!found) throw new Error(`${record.Name} has no default individual activity`);
  return found;
}

export function teamActivity(record = event) {
  const found = record.Activities.find((activity) => activity.AudienceTypeId === 'TEAM');
  if (!found) throw new Error(`${record.Name} has no team activity`);
  return found;
}
