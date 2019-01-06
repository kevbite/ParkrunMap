import React from 'react';
import { connect } from 'react-redux';

class NavSpinner extends React.Component {
  render() {

    console.log(this.props.isLoading);
    if(!this.props.isLoading){
      return null;
    }

    return (<div class="spinner-border text-secondary mx-auto" role="status">
              <span class="sr-only">Loading...</span>
            </div>);
  }
}


function mapStateToProps(state) {
  console.dir(state);
  return {
    isLoading: state.parkruns.isLoading
  }
}

export default connect(
  mapStateToProps
)(NavSpinner);
