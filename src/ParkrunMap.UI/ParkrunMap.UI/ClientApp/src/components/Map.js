import React from 'react';
import { connect } from 'react-redux';
import { compose, withProps } from "recompose"
import {
  withScriptjs,
  withGoogleMap,
  GoogleMap,
  Marker,
  InfoWindow
} from "react-google-maps"

class ParkrunMarker extends React.Component {
  constructor(props) {
    super(props);

    this.toggle = this.toggle.bind(this);
    this.onParkrunMarkerClick = this.onParkrunMarkerClick.bind(this);
    this.state = {
      showInformation: false
    };
  }

  toggle() {
    this.setState({
      showInformation: !this.state.showInformation
    });
  }

  onParkrunMarkerClick(parkrun) {
    this.toggle();
  }

  getPinIcon() {
    const greenPinIcon = "https://maps.google.com/mapfiles/ms/icons/green-dot.png";
    const orangePinIcon = "https://maps.google.com/mapfiles/ms/icons/orange-dot.png";
    const redPinIcon = "https://maps.google.com/mapfiles/ms/icons/red-dot.png";

    function calculateNextSaturday(){
      var now = new Date();    
      now.setDate(now.getDate() + (6+(7-now.getDay())) % 7);
      now.setHours(0,0,0,0);

      return now;
  }

  const nextSaturday = calculateNextSaturday();
  
  const cancellations = this.props.parkrun.cancellations.map(x =>{
    var d = new Date(x.date);
    d.setHours(0,0,0,0);
    return d;
  });
  if(cancellations.findIndex(x => x.valueOf() === nextSaturday.valueOf()) >= 0){
    return redPinIcon;
  }else if(cancellations.length){
    return orangePinIcon;
  }

  return greenPinIcon;
  };

  render() {
    return (
      <Marker
        key={this.props.parkrun.name}
        position={{ lat: this.props.parkrun.lat, lng: this.props.parkrun.lon }}
        icon={this.getPinIcon()}
        onClick={this.onParkrunMarkerClick}>
        {this.state.showInformation &&
          <InfoWindow
            onCloseClick={this.toggle}>
            <div>
              <strong>{this.props.parkrun.name}</strong>
              <ul>
                <li><a href={this.props.parkrun.uri}>Website</a></li>
                {this.props.parkrun.cancellations.length > 0 &&
                   <li>
                     <div class="warning">Cancellations</div>
                     <ul>
                     {this.props.parkrun.cancellations.map(x => <li>{new Date(x.date).toDateString()}</li>)}
                     </ul>
                   </li>
                }
              </ul>
            </div>
          </InfoWindow>}

      </Marker>);
  }

}

class Map extends React.Component {

  render() {
    return (
      <GoogleMap
        defaultZoom={12}
        defaultCenter={{ lat: this.props.center.latitude, lng: this.props.center.longitude }}
        center={{ lat: this.props.center.latitude, lng: this.props.center.longitude }}
      >
        {this.props.parkruns &&
          this.props.parkruns.map(parkrun => <ParkrunMarker key={parkrun.name} parkrun={parkrun} />)}
      </GoogleMap>
    );
  }
}

const ComposedComponent = compose(
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
)(Map);

export default connect()(ComposedComponent);
