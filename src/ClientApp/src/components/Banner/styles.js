import styled from 'styled-components';

export const BannerWrapper = styled.div`
  text-align: ${props => (props.center ? 'center' : 'initial')};
  background: ${props => props.background};
  color: ${props => props.color};
  padding: 2rem;
  box-shadow: inset 0 8px 8px -8px rgba(0, 0, 0, 0.3), inset 0 -8px 8px -8px rgba(0, 0, 0, 0.3);

  h1 {
    text-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
    font-weight: 700 !important;
    font-size: 3.5rem;
    padding-bottom: 0.5rem;
    margin-bottom: 0;
  }

  p {
    color: #fff;
    font-size: 1.5rem;
    font-weight: 300 !important;
    margin-bottom: 0;
    padding-bottom: 0;
  }
`;
