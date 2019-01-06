import React from 'react';
import { connect } from 'react-redux';

class NavSpinner extends React.Component {
  render() {

    if(!this.props.isLoading){
      return null;
    }

    return (<div className="spinner-border text-secondary mx-auto" role="status">
              <span className="sr-only">Loading...</span>
            </div>);
  }
}


function mapStateToProps(state) {
  return {
    isLoading: state.parkruns.isLoading
  }
}

export default connect(
  mapStateToProps
)(NavSpinner);
