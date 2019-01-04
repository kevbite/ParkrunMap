import React from 'react';
import { connect } from 'react-redux';
import { compose, withProps } from "recompose";
import {
  withScriptjs,
  withGoogleMap,
  GoogleMap
} from "react-google-maps";
import ParkrunMarker from './ParkrunMarker';

class Map extends React.Component {

  render() {
    return (
      <GoogleMap
        defaultZoom={12}
        defaultCenter={{ lat: this.props.center.latitude, lng: this.props.center.longitude }}
        center={{ lat: this.props.center.latitude, lng: this.props.center.longitude }}
        options={{streetViewControl:false}}
      >
        {this.props.parkruns &&
          this.props.parkruns.map(parkrun => <ParkrunMarker key={parkrun.name} parkrun={parkrun} />)}
      </GoogleMap>
    );
  }
}

const ComposedComponent = compose(
  withProps({
    googleMapURL: "https://maps.googleapis.com/maps/api/js?key=AIzaSyAvs9kC7xNv1oQbkjzeC106u8s43r5HoXA&v=3.exp&libraries=geometry,drawing,places",
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
)(Map);

export default connect()(ComposedComponent);
