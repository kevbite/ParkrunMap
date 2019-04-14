export default function selectMetadata(filters) {

  const keywords = "Parkrun,Running,Events,Map,Cancelled";

  if (filters.wheelchairFriendly) {
    return {
      title: "Wheelchair friendly parkruns",
      description: "Find wheelchair friendly parkruns",
      keywords: `${keywords},wheelchair,disabled,accessibility`
    }
  }
  
  if (filters.buggyFriendly) {
    return {
      title: "Buggy friendly parkruns",
      description: "Find buggy friendly parkruns",
      keywords: `${keywords},buggy,baby,prams,pushchairs`
    }
  }

  return {
    keywords: keywords
  }
}