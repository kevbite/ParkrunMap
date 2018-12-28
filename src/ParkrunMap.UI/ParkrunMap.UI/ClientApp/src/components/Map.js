import React from 'react';
import { connect } from 'react-redux';
import { compose, withProps } from "recompose"
import { withScriptjs, withGoogleMap, GoogleMap, Marker } from "react-google-maps"

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
    defaultCenter={{ lat: props.defaultCenter.latitude, lng: props.defaultCenter.longitude}}
  >
    {props.parkruns && props.parkruns.map(parkrun => <Marker key={parkrun.name} position={{ lat: parkrun.lat, lng: parkrun.lon }} onClick={props.onParkrunMarkerClick} />)}
  </GoogleMap>
);

export default connect()(ParkrunMap);
