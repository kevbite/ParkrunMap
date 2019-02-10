import React, { createRef } from 'react';
import { compose, withProps } from "recompose";
import {
  withScriptjs,
  withGoogleMap,
  GoogleMap
} from "react-google-maps";
import ParkrunMarker from './ParkrunMarker';

class Map extends React.Component {
  constructor(props) {
    super(props);

    this.mapRef = createRef();
    this.state = {
      userLocation: {
        lat: props.userLocation.latitude,
        lng: props.userLocation.longitude
      },
      center: {
        lat: props.userLocation.latitude,
        lng: props.userLocation.longitude
      }
    };
  }

  componentWillReceiveProps(nextProps) {
    if (this.state.userLocation.lat !== nextProps.userLocation.latitude
      || this.state.userLocation.lng !== nextProps.userLocation.longitude) {
      const state = {
        userLocation: {
          lat: nextProps.userLocation.latitude,
          lng: nextProps.userLocation.longitude
        },
        center: {
          lat: nextProps.userLocation.latitude,
          lng: nextProps.userLocation.longitude
        }
      }
      this.setState(state);
    }
  }

  onIdle = () => {
    const mapBounds = this.mapRef.current.getBounds();
    const ne = mapBounds.getNorthEast();
    const sw = mapBounds.getSouthWest();

    const bounds = {
      bottomLeft: { lat: sw.lat(), lon: sw.lng() },
      topRight: { lat: ne.lat(), lon: ne.lng() }
    };

    const center = this.mapRef.current.getCenter();

    if (!this.state.bounds || JSON.stringify(bounds) !== JSON.stringify(this.state.bounds)) {
      this.setState({ bounds, center: { lat: center.lat(), lng: center.lng() } }, () => this.props.onBoundsChanged(this.state.bounds))
    }
  }

  onChange = ({ center, zoom }) => {
    this.setState({
      center: center
    });
  }

  render() {
    return (
      <GoogleMap
        ref={this.mapRef}
        onIdle={this.onIdle}
        onChange={this.onChange}
        defaultZoom={12}
        defaultCenter={this.state.center}
        center={this.state.center}
        options={{ streetViewControl: false }}
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

export default ComposedComponent;
