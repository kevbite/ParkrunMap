

export default function selectParkruns(parkruns, filters) {

  if (!filters.wheelchairFriendly && !filters.buggyFriendly) {
    return parkruns;
  }

  const filteredParkruns = parkruns.filter(parkrun => {

    const selectedWhen = [
      filters.wheelchairFriendly && parkrun.features.wheelchairFriendly,
      filters.buggyFriendly && parkrun.features.buggyFriendly
    ];

    return selectedWhen.some(x => x);
  });

  return filteredParkruns;
}