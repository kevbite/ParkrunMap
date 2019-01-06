import React from 'react';
import {
  Marker,
  InfoWindow
} from "react-google-maps";
import {
  Button,
  CardBody,
  CardSubtitle,
  CardTitle,
  CardText
} from 'reactstrap';

class ParkrunMarker extends React.Component {

  state = {
    showInformation: false
  };

  toggle = () => {
    this.setState({
      showInformation: !this.state.showInformation
    });
  }

  onParkrunMarkerClick = () => {
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
             <CardBody>
                <CardTitle>{this.props.parkrun.name}</CardTitle>
                <CardText></CardText>
                {this.props.parkrun.cancellations.length > 0 &&
                  <div>
                    <CardSubtitle>Cancellations</CardSubtitle>
                    <ul>
                      {this.props.parkrun.cancellations.map(x => <li>{new Date(x.date).toDateString()}</li>)}
                    </ul>
                  </div>
                }
                <Button color="info" href={this.props.parkrun.uri}>Website</Button>
                {' '}
                {this.props.parkrun.cancellations.length > 0 && 
                  <Button color="warning" href="https://www.parkrun.org.uk/cancellations/">Cancellations</Button>
                }
                {' '}
                <Button color="secondary" onClick={this.toggle}>Close</Button>
            </CardBody>
          </InfoWindow>}

      </Marker>);
  }

}

export default ParkrunMarker;