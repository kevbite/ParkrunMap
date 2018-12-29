import React from 'react';
import { connect } from 'react-redux';
import { compose, withProps } from "recompose"
import { withScriptjs, withGoogleMap, GoogleMap, Marker } from "react-google-maps"

function getPinIcon(parkrun){
  const greenPinIcon = "https://maps.google.com/mapfiles/ms/icons/green-dot.png";
  const orangePinIcon = "https://maps.google.com/mapfiles/ms/icons/orange-dot.png";
  const redPinIcon = "https://maps.google.com/mapfiles/ms/icons/red-dot.png";
  
  // Everything is green for moment.
  return greenPinIcon;
};

const ParkrunMap = compose(
  withProps({
    googleMapURL: "https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=geometry,drawing,places",
    loadingElement: <div style={{ height: `100%` }} />,
    containerElement: <div style={{
      position: `absolute`,
      width: `100%`,
      top: `56px`,
      bottom: `0px`
    }} />,
    mapElement: <div style={{ height: `100%` }} />,
  }),
  withScriptjs,
  withGoogleMap
)((props) =>
  <GoogleMap
    defaultZoom={12}
    defaultCenter={{ lat: props.center.latitude, lng: props.center.longitude}}
    center={{ lat: props.center.latitude, lng: props.center.longitude}}
  >
    {props.parkruns &&
      props.parkruns.map(parkrun => <Marker key={parkrun.name} position={{ lat: parkrun.lat, lng: parkrun.lon }} icon={getPinIcon(parkrun)} onClick={props.onParkrunMarkerClick} />)}
  </GoogleMap>
);

export default connect()(ParkrunMap);
